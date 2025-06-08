using System;

namespace Jawbone.Sockets.Windows;

sealed class WindowsTcpClientV4 : ITcpClient<IpAddressV4>
{
    private readonly nuint _fd;

    public IpEndpoint<IpAddressV4> Origin { get; }

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }
    public bool HungUp { get; private set; }

    public WindowsTcpClientV4(nuint fd, IpEndpoint<IpAddressV4> origin)
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

    public TransferResult Receive(Span<byte> buffer, int timeoutInMilliseconds)
    {
        var milliseconds = int.Max(0, timeoutInMilliseconds);
        var pfd = new WsaPollFd { Fd = _fd, Events = Poll.In };

    retry:
        var start = Environment.TickCount64;
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
                        Sys.Throw(error, ExceptionMessages.ReceiveData);
                    if (HandleInterruptOnReceive == InterruptHandling.Abort)
                        return new(SocketResult.Interrupt);
                    goto retryReceive;
                }

                return new(readResult);
            }

            if ((pfd.REvents & Poll.Err) != 0)
                ThrowExceptionFor.PollSocketError();

            return new(0);
        }
        else if (pollResult == -1)
        {
            var error = Sys.WsaGetLastError();
            if (!Error.IsInterrupt(error) || HandleInterruptOnReceive == InterruptHandling.Error)
            {
                Sys.Throw(error, ExceptionMessages.Poll);
            }
            else if (HandleInterruptOnReceive == InterruptHandling.Abort)
            {
                return new(SocketResult.Interrupt);
            }
            else
            {
                var elapsed = (int)(Environment.TickCount64 - start);
                milliseconds = int.Max(0, milliseconds - elapsed);
                goto retry;
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

    public IpEndpoint<IpAddressV4> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out var address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return address.GetV4(addressLength);
    }

    public static WindowsTcpClientV4 Connect(
        IpEndpoint<IpAddressV4> ipEndpoint,
        SocketOptions socketOptions)
    {
        var fd = Sys.Socket(Af.INet, Sock.Stream, 0);

        if (fd == Sys.InvalidSocket)
            Sys.Throw(ExceptionMessages.OpenSocket);

        try
        {
            Tcp.SetNoDelay(fd, !socketOptions.All(SocketOptions.DisableTcpNoDelay));
            var addr = SockAddrIn.FromEndpoint(ipEndpoint);
            var result = Sys.ConnectV4(fd, addr, SockAddrIn.Len);
            if (result == -1)
            {
                var error = Sys.WsaGetLastError();
                Sys.Throw(error, $"Failed to connect to {ipEndpoint}.");
            }

            return new WindowsTcpClientV4(fd, ipEndpoint);
        }
        catch
        {
            _ = Sys.CloseSocket(fd);
            throw;
        }
    }
}
