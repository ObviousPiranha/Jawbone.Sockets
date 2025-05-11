using System;

namespace Jawbone.Sockets;

public interface ITcpListener<TAddress> : IDisposable
    where TAddress : unmanaged, IAddress<TAddress>
{
    InterruptHandling HandleInterruptOnAccept { get; set; }
    bool WasInterrupted { get; }

    ITcpClient<TAddress>? Accept(int timeoutInMilliseconds);
    Endpoint<TAddress> GetSocketName();
}
