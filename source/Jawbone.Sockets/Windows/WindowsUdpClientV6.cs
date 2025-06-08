using System;
using System.Diagnostics;

namespace Jawbone.Sockets.Windows;

sealed class WindowsUdpClientV6 : IUdpClient<IpAddressV6>
{
    private readonly nuint _fd;
    private SockAddrStorage _address;

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }

    public IpEndpoint<IpAddressV6> Origin { get; }

    public WindowsUdpClientV6(nuint fd, IpEndpoint<IpAddressV6> origin)
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

    public IpEndpoint<IpAddressV6> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out _address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return _address.GetV6(addressLength);
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
            if ((pfd.REvents & Poll.In) != 0)
            {
            retryReceive:
                var addressLength = SockAddrStorage.Len;
                var receiveResult = Sys.RecvFrom(
                    _fd,
                    out buffer.GetPinnableReference(),
                    buffer.Length,
                    0,
                    out _address,
                    ref addressLength);

                if (receiveResult == -1)
                {
                    var error = Sys.WsaGetLastError();
                    if (!Error.IsInterrupt(error) || HandleInterruptOnReceive == InterruptHandling.Error)
                        Sys.Throw(error, ExceptionMessages.ReceiveData);
                    if (HandleInterruptOnReceive == InterruptHandling.Abort)
                        return new(SocketResult.Interrupt);
                    goto retryReceive;
                }

                var origin = _address.GetV6(addressLength);
                Debug.Assert(origin == Origin);
                return new((int)receiveResult);
            }

            if ((pfd.REvents & Poll.Err) != 0)
                ThrowExceptionFor.PollSocketError();
            ThrowExceptionFor.BadPollState();
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
        var result = Sys.Send(
            _fd,
            message.GetPinnableReference(),
            message.Length,
            0);

        if (result == -1)
        {
            var error = Sys.WsaGetLastError();
            if (!Error.IsInterrupt(error) || HandleInterruptOnSend == InterruptHandling.Error)
                Sys.Throw(error, ExceptionMessages.SendDatagram);
            if (HandleInterruptOnSend == InterruptHandling.Abort)
                return new(SocketResult.Interrupt);
            goto retry;
        }

        return new(result);
    }

    public static WindowsUdpClientV6 Connect(
        IpEndpoint<IpAddressV6> ipEndpoint,
        SocketOptions socketOptions)
    {
        var fd = CreateSocket();

        try
        {
            var sa = SockAddrIn6.FromEndpoint(ipEndpoint);
            var result = Sys.ConnectV6(fd, sa, SockAddrIn6.Len);
            if (result == -1)
            {
                var error = Sys.WsaGetLastError();
                Sys.Throw(error, $"Failed to connect to {ipEndpoint}.");
            }

            return new WindowsUdpClientV6(fd, ipEndpoint);
        }
        catch
        {
            _ = Sys.CloseSocket(fd);
            throw;
        }
    }

    private static nuint CreateSocket()
    {
        var fd = Sys.Socket(Af.INet6, Sock.DGram, IpProto.Udp);

        if (fd == Sys.InvalidSocket)
            Sys.Throw(ExceptionMessages.OpenSocket);

        return fd;
    }
}
