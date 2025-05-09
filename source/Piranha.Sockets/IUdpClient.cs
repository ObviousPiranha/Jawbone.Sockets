using System;

namespace Piranha.Sockets;

public interface IUdpClient<TAddress> : IDisposable
    where TAddress : unmanaged, IAddress<TAddress>
{
    Endpoint<TAddress> Origin { get; }
    int Send(ReadOnlySpan<byte> message);
    int? Receive(Span<byte> buffer, TimeSpan timeout);
    Endpoint<TAddress> GetSocketName();
}
