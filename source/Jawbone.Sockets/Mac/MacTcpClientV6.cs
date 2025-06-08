using System;

namespace Jawbone.Sockets.Mac;

sealed class MacTcpClientV6 : ITcpClient<IpAddressV6>
{
    private readonly int _fd;

    public IpEndpoint<IpAddressV6> Origin { get; }

    public InterruptHandling HandleInterruptOnSend { get; set; }
    public InterruptHandling HandleInterruptOnReceive { get; set; }
    public bool HungUp { get; private set; }

    public MacTcpClientV6(int fd, IpEndpoint<IpAddressV6> origin)
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

            if ((pfd.REvents & Poll.Hup) != 0)
            {
                HungUp = true;
            }

            if ((pfd.REvents & Poll.In) != 0)
            {
            retryReceive:
                var readResult = Sys.Read(
                    _fd,
                    out buffer.GetPinnableReference(),
                    (nuint)buffer.Length);

                if (readResult == -1)
                {
                    var errNo = Sys.ErrNo();
                    if (!Error.IsInterrupt(errNo) || HandleInterruptOnReceive == InterruptHandling.Error)
                        Sys.Throw(errNo, ExceptionMessages.ReceiveData);
                    if (HandleInterruptOnReceive == InterruptHandling.Abort)
                        return new(SocketResult.Interrupt);
                    goto retryReceive;
                }

                return new((int)readResult);
            }

            if ((pfd.REvents & Poll.Err) != 0)
            {
                ThrowExceptionFor.PollSocketError();
            }

            return new(0);
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
        var writeResult = Sys.Write(
            _fd,
            message.GetPinnableReference(),
            (nuint)message.Length);

        if (writeResult == -1)
        {
            var errNo = Sys.ErrNo();
            if (!Error.IsInterrupt(errNo) || HandleInterruptOnSend == InterruptHandling.Error)
                Sys.Throw(ExceptionMessages.SendStream);
            if (HandleInterruptOnSend == InterruptHandling.Abort)
                return new(SocketResult.Interrupt);
            goto retry;
        }

        return new((int)writeResult);
    }

    public IpEndpoint<IpAddressV6> GetSocketName()
    {
        var addressLength = SockAddrStorage.Len;
        var result = Sys.GetSockName(_fd, out var address, ref addressLength);
        if (result == -1)
            Sys.Throw(ExceptionMessages.GetSocketName);
        return address.GetV6(addressLength);
    }

    public static MacTcpClientV6 Connect(
        IpEndpoint<IpAddressV6> ipEndpoint,
        SocketOptions socketOptions)
    {
        int fd = Sys.Socket(Af.INet6, Sock.Stream, 0);

        if (fd == -1)
            Sys.Throw(ExceptionMessages.OpenSocket);

        try
        {
            Tcp.SetNoDelay(fd, !socketOptions.All(SocketOptions.DisableTcpNoDelay));
            var addr = SockAddrIn6.FromEndpoint(ipEndpoint);
            var connectResult = Sys.ConnectV6(fd, addr, SockAddrIn6.Len);
            if (connectResult == -1)
            {
                var errNo = Sys.ErrNo();
                Sys.Throw(errNo, $"Failed to connect to {ipEndpoint}.");
            }

            return new MacTcpClientV6(fd, ipEndpoint);
        }
        catch
        {
            _ = Sys.Close(fd);
            throw;
        }
    }
}
