using System;

namespace Jawbone.Sockets;

public interface ITcpListener<TAddress> : IDisposable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    InterruptHandling HandleInterruptOnAccept { get; set; }
    bool WasInterrupted { get; }

    ITcpClient<TAddress>? Accept(int timeoutInMilliseconds);
    IpEndpoint<TAddress> GetSocketName();
}
