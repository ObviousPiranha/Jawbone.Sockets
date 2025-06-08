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
