using System;

namespace Piranha.Sockets;

public interface ITcpListener<TAddress> : IDisposable
    where TAddress : unmanaged, IAddress<TAddress>
{
    InterruptHandling HandleInterruptOnAccept { get; set; }
    bool WasInterrupted { get; }

    ITcpClient<TAddress>? Accept(TimeSpan timeout);
    Endpoint<TAddress> GetSocketName();
}
