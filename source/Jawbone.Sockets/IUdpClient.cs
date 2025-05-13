using System;

namespace Jawbone.Sockets;

public interface IUdpClient<TAddress> : IDisposable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    InterruptHandling HandleInterruptOnSend { get; set; }
    InterruptHandling HandleInterruptOnReceive { get; set; }

    IpEndpoint<TAddress> Origin { get; }
    TransferResult Send(ReadOnlySpan<byte> message);
    TransferResult Receive(Span<byte> buffer, int timeoutInMilliseconds);
    IpEndpoint<TAddress> GetSocketName();
}
