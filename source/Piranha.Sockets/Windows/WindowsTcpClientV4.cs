using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Piranha.Sockets.Windows;

sealed class WindowsTcpClientV4 : ITcpClient<AddressV4>
{
    private readonly nuint _fd;

    public Endpoint<AddressV4> Origin { get; }

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }
    public bool HungUp { get; private set; }

    public WindowsTcpClientV4(nuint fd, Endpoint<AddressV4> origin)
    {
        _fd = fd;
        Origin = origin;
    }

    public void Dispose()
    {
        var result = Sys.CloseSocket(_fd);
        if (result == -1)
            Sys.Throw(ExceptionMessages.CloseSocket);
    }

    public TransferResult Receive(Span<byte> buffer, TimeSpan timeout)
    {
        var milliseconds = Core.GetMilliseconds(timeout);
        var pfd = new WsaPollFd { Fd = _fd, Events = Poll.In };

    retry:
        var start = Stopwatch.GetTimestamp();
        var pollResult = Sys.WsaPoll(ref pfd, 1, milliseconds);

        if (0 < pollResult)
        {
            ObjectDisposedException.ThrowIf((pfd.REvents & Poll.Nval) != 0, this);

            if ((pfd.REvents & Poll.Hup) != 0)
            {
                HungUp = true;
            }

            if ((pfd.REvents & Poll.In) != 0)
            {
            retryReceive:
                var readResult = Sys.Recv(
                    _fd,
                    out buffer.GetPinnableReference(),
                    buffer.Length,
                    0);

                if (readResult == -1)
                {
                    var error = Sys.WsaGetLastError();
                    if (!Error.IsInterrupt(error) || HandleInterruptOnReceive == InterruptHandling.Error)
                        Sys.Throw(ExceptionMessages.ReceiveData);
                    if (HandleInterruptOnReceive == InterruptHandling.Abort)
                        return new(SocketResult.Interrupt);
                    goto retryReceive;
                }

                return new(readResult);
            }

            if ((pfd.REvents & Poll.Err) != 0)
            {
                ThrowExceptionFor.PollSocketError();
            }

            return new(0);
        }
        else if (pollResult < 0)
        {
            var error = Sys.WsaGetLastError();
            if (!Error.IsInterrupt(error) || HandleInterruptOnReceive == InterruptHandling.Error)
            {
                Sys.Throw(ExceptionMessages.Poll);
            }
            else if (HandleInterruptOnReceive != InterruptHandling.Abort)
            {
                var elapsed = Stopwatch.GetElapsedTime(start);
                milliseconds = Core.GetMilliseconds(timeout - elapsed);
                goto retry;
            }
            else
            {
                return new(SocketResult.Interrupt);
            }
        }

        return new(SocketResult.Timeout);
    }

    public TransferResult Send(ReadOnlySpan<byte> message)
    {
    retry:
        var writeResult = Sys.Send(
            _fd,
            message.GetPinnableReference(),
            message.Length,
            0);

        if (writeResult == -1)
        {
            var error = Sys.WsaGetLastError();
            if (!Error.IsInterrupt(error) || HandleInterruptOnSend == InterruptHandling.Error)
                Sys.Throw(ExceptionMessages.SendStream);
            if (HandleInterruptOnSend == InterruptHandling.Abort)
                return new(SocketResult.Interrupt);
            goto retry;
        }

        return new(writeResult);
    }

    public Endpoint<AddressV4> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out var address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return address.GetV4(addressLength);
    }

    public static WindowsTcpClientV4 Connect(Endpoint<AddressV4> endpoint)
    {
        var fd = Sys.Socket(Af.INet, Sock.Stream, 0);

        if (fd == Sys.InvalidSocket)
            Sys.Throw(ExceptionMessages.OpenSocket);

        try
        {
            Tcp.SetNoDelay(fd);
            var addr = SockAddrIn.FromEndpoint(endpoint);
            var result = Sys.ConnectV4(fd, addr, SockAddrIn.Len);
            if (result == -1)
            {
                var error = Sys.WsaGetLastError();
                Sys.Throw(error, $"Failed to connect to {endpoint}.");
            }

            return new WindowsTcpClientV4(fd, endpoint);
        }
        catch
        {
            _ = Sys.CloseSocket(fd);
            throw;
        }
    }
}
