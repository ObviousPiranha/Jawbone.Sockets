using System;

namespace Jawbone.Sockets;

public interface ITcpClient<TAddress> : IDisposable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    InterruptHandling HandleInterruptOnSend { get; set; }
    InterruptHandling HandleInterruptOnReceive { get; set; }
    bool HungUp { get; }

    IpEndpoint<TAddress> Origin { get; }
    TransferResult Send(ReadOnlySpan<byte> message);
    TransferResult Receive(Span<byte> buffer, int timeoutInMilliseconds);
    IpEndpoint<TAddress> GetSocketName();
}
