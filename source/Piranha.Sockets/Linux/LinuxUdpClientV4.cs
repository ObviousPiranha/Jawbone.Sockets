using System;
using System.Diagnostics;

namespace Piranha.Sockets.Linux;

sealed class LinuxUdpClientV4 : IUdpClient<AddressV4>
{
    private readonly int _fd;
    private SockAddrStorage _address;

    public Endpoint<AddressV4> Origin { get; }

    public LinuxUdpClientV4(int fd, Endpoint<AddressV4> origin)
    {
        _fd = fd;
        Origin = origin;
    }

    public void Dispose()
    {
        var result = Sys.Close(_fd);
        if (result == -1)
            Sys.Throw("Unable to close socket.");
    }

    public Endpoint<AddressV4> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out _address, ref addressLength);
        if (result == -1)
            Sys.Throw("Unable to get socket name.");
        return _address.GetV4(addressLength);
    }

    public int? Receive(Span<byte> buffer, TimeSpan timeout)
    {
        var milliseconds = Core.GetMilliseconds(timeout);
        var pfd = new PollFd { Fd = _fd, Events = Poll.In };
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
                    Sys.Throw("Unable to receive data.");

                var origin = _address.GetV4(addressLength);
                Debug.Assert(origin == Origin);
                return (int)receiveResult;
            }
        }
        else if (pollResult < 0)
        {
            Sys.Throw("Unable to poll socket.");
        }
        return null;
    }

    public int Send(ReadOnlySpan<byte> message)
    {
        var result = Sys.Send(
            _fd,
            message.GetPinnableReference(),
            (nuint)message.Length,
            0);

        if (result == -1)
            Sys.Throw("Unable to send data.");

        return (int)result;
    }

    public static LinuxUdpClientV4 Connect(Endpoint<AddressV4> endpoint)
    {
        var fd = CreateSocket();

        try
        {
            var sa = SockAddrIn.FromEndpoint(endpoint);
            var result = Sys.ConnectV4(fd, sa, SockAddrIn.Len);
            if (result == -1)
            {
                var errNo = Sys.ErrNo();
                Sys.Throw(errNo, $"Failed to connect to {endpoint}.");
            }

            return new LinuxUdpClientV4(fd, endpoint);
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
            Sys.Throw("Unable to open socket.");

        return fd;
    }
}
