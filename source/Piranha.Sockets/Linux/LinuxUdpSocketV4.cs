using System;
using System.Diagnostics;

namespace Piranha.Sockets.Linux;

sealed class LinuxUdpSocketV4 : IUdpSocket<AddressV4>
{
    private readonly int _fd;
    private SockAddrStorage _address;

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }

    private LinuxUdpSocketV4(int fd)
    {
        _fd = fd;
    }

    public void Dispose()
    {
        var result = Sys.Close(_fd);
        if (result == -1)
            Sys.Throw(ExceptionMessages.CloseSocket);
    }

    public unsafe TransferResult Send(ReadOnlySpan<byte> message, Endpoint<AddressV4> destination)
    {
        var sa = SockAddrIn.FromEndpoint(destination);

    retry:
        var result = Sys.SendToV4(
            _fd,
            message.GetPinnableReference(),
            (nuint)message.Length,
            0,
            sa,
            SockAddrIn.Len);

        if (result == -1)
        {
            var errNo = Sys.ErrNo();
            if (!Error.IsInterrupt(errNo) || HandleInterruptOnSend == InterruptHandling.Error)
                Sys.Throw(errNo, ExceptionMessages.SendDatagram);
            if (HandleInterruptOnSend != InterruptHandling.Abort)
                goto retry;
            return new(SocketResult.Interrupt);
        }

        return new((int)result);
    }

    public unsafe TransferResult Receive(
        Span<byte> buffer,
        TimeSpan timeout,
        out Endpoint<AddressV4> origin)
    {
        var milliseconds = Core.GetMilliseconds(timeout);
        var pfd = new PollFd { Fd = _fd, Events = Poll.In };

    retry:
        var start = Stopwatch.GetTimestamp();
        var pollResult = Sys.Poll(ref pfd, 1, milliseconds);

        if (0 < pollResult)
        {
            if ((pfd.REvents & Poll.In) != 0)
            {
                var addressLength = SockAddrStorage.Len;
                var receiveResult = Sys.RecvFrom(
                    _fd,
                    out buffer.GetPinnableReference(),
                    (nuint)buffer.Length,
                    0,
                    out _address,
                    ref addressLength);

                if (receiveResult == -1)
                    Sys.Throw(ExceptionMessages.ReceiveData);

                origin = _address.GetV4(addressLength);
                return new((int)receiveResult);
            }
            else
            {
                throw CreateExceptionFor.BadPoll();
            }
        }
        else if (pollResult == -1)
        {
            var errNo = Sys.ErrNo();
            if (!Error.IsInterrupt(errNo) || HandleInterruptOnReceive == InterruptHandling.Error)
            {
                Sys.Throw(errNo, ExceptionMessages.Poll);
            }
            else if (HandleInterruptOnReceive != InterruptHandling.Abort)
            {
                var elapsed = Stopwatch.GetElapsedTime(start);
                milliseconds = Core.GetMilliseconds(timeout - elapsed);
                goto retry;
            }
            else
            {
                origin = default;
                return new(SocketResult.Interrupt);
            }
        }

        origin = default;
        return new(SocketResult.Timeout);
    }

    public unsafe Endpoint<AddressV4> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out _address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return _address.GetV4(addressLength);
    }

    public static LinuxUdpSocketV4 Create()
    {
        var fd = CreateSocket();
        return new LinuxUdpSocketV4(fd);
    }

    public static LinuxUdpSocketV4 Bind(Endpoint<AddressV4> endpoint)
    {
        var fd = CreateSocket();

        try
        {
            So.SetReuseAddr(fd);
            var sa = SockAddrIn.FromEndpoint(endpoint);
            var bindResult = Sys.BindV4(fd, sa, SockAddrIn.Len);

            if (bindResult == -1)
            {
                var errNo = Sys.ErrNo();
                Sys.Throw(errNo, $"Failed to bind socket to address {endpoint}.");
            }

            return new LinuxUdpSocketV4(fd);
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
