using System;
using System.Diagnostics;

namespace Jawbone.Sockets.Linux;

sealed class LinuxUdpClientV4 : IUdpClient<IpAddressV4>
{
    private readonly int _fd;
    private SockAddrStorage _address;

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }

    public IpEndpoint<IpAddressV4> Origin { get; }

    public LinuxUdpClientV4(int fd, IpEndpoint<IpAddressV4> origin)
    {
        _fd = fd;
        Origin = origin;
    }

    public void Dispose()
    {
        var result = Sys.Close(_fd);
        if (result == -1)
            Sys.Throw(ExceptionMessages.CloseSocket);
    }

    public IpEndpoint<IpAddressV4> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out _address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return _address.GetV4(addressLength);
    }

    public TransferResult Receive(Span<byte> buffer, int timeoutInMilliseconds)
    {
        var milliseconds = int.Max(0, timeoutInMilliseconds);
        var pfd = new PollFd { Fd = _fd, Events = Poll.In };

    retry:
        var start = Environment.TickCount64;
        var pollResult = Sys.Poll(ref pfd, 1, milliseconds);

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
                    (nuint)buffer.Length,
                    0,
                    out _address,
                    ref addressLength);

                if (receiveResult == -1)
                {
                    var errNo = Sys.ErrNo();
                    if (!Error.IsInterrupt(errNo) || HandleInterruptOnReceive == InterruptHandling.Error)
                        Sys.Throw(errNo, ExceptionMessages.ReceiveData);
                    if (HandleInterruptOnReceive == InterruptHandling.Abort)
                        return new(SocketResult.Interrupt);
                    goto retryReceive;
                }

                var origin = _address.GetV4(addressLength);
                Debug.Assert(origin == Origin);
                return new((int)receiveResult);
            }

            if ((pfd.REvents & Poll.Err) != 0)
                ThrowExceptionFor.PollSocketError();
            ThrowExceptionFor.BadPollState();
        }
        else if (pollResult == -1)
        {
            var errNo = Sys.ErrNo();
            if (!Error.IsInterrupt(errNo) || HandleInterruptOnReceive == InterruptHandling.Error)
            {
                Sys.Throw(errNo, ExceptionMessages.Poll);
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
            (nuint)message.Length,
            0);

        if (result == -1)
        {
            var errNo = Sys.ErrNo();
            if (!Error.IsInterrupt(errNo) || HandleInterruptOnSend == InterruptHandling.Error)
                Sys.Throw(errNo, ExceptionMessages.SendDatagram);
            if (HandleInterruptOnSend == InterruptHandling.Abort)
                return new(SocketResult.Interrupt);
            goto retry;
        }

        return new((int)result);
    }

    public static LinuxUdpClientV4 Connect(
        IpEndpoint<IpAddressV4> ipEndpoint,
        SocketOptions socketOptions)
    {
        var fd = CreateSocket();

        try
        {
            var sa = SockAddrIn.FromEndpoint(ipEndpoint);
            var result = Sys.ConnectV4(fd, sa, SockAddrIn.Len);
            if (result == -1)
            {
                var errNo = Sys.ErrNo();
                Sys.Throw(errNo, $"Failed to connect to {ipEndpoint}.");
            }

            return new LinuxUdpClientV4(fd, ipEndpoint);
        }
        catch
        {
            _ = Sys.Close(fd);
            throw;
        }
    }

    private static int CreateSocket()
    {
        int fd = Sys.Socket(Af.INet, Sock.DGram, IpProto.Udp);

        if (fd == -1)
            Sys.Throw(ExceptionMessages.OpenSocket);

        return fd;
    }
}
