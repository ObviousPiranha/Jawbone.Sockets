using System;

namespace Jawbone.Sockets;

public interface IUdpSocket<TAddress> : IDisposable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    InterruptHandling HandleInterruptOnSend { get; set; }
    InterruptHandling HandleInterruptOnReceive { get; set; }

    TransferResult Send(
        ReadOnlySpan<byte> message,
        IpEndpoint<TAddress> destination);
    TransferResult Receive(
        Span<byte> buffer,
        int timeoutInMilliseconds,
        out IpEndpoint<TAddress> origin);
    IpEndpoint<TAddress> GetSocketName();
}

public static class UdpSocketExtensions
{
    public static void Receive<TAddress>(
        this IUdpSocket<TAddress> udpSocket,
        ref Span<byte> buffer,
        TimeSpan timeout,
        out IpEndpoint<TAddress> origin)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        var result = udpSocket.Receive(buffer, Core.GetMilliseconds(timeout), out origin);
        if (result.Result == SocketResult.Timeout)
            throw new TimeoutException();
        buffer = buffer[..result.Count];
    }

    public static void Receive<TAddress>(
        this IUdpSocket<TAddress> udpSocket,
        ref Span<byte> buffer,
        TimeSpan timeout)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        var result = udpSocket.Receive(buffer, Core.GetMilliseconds(timeout), out _);
        if (result.Result == SocketResult.Timeout)
            throw new TimeoutException();
        buffer = buffer[..result.Count];
    }
}
