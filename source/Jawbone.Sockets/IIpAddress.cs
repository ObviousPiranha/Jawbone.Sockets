using System;
using System.Text;

namespace Jawbone.Sockets;

public interface IIpAddress
{
    bool IsDefault { get; }
    bool IsLinkLocal { get; }
    bool IsLoopback { get; }
    void AppendTo(StringBuilder builder);
}

public interface IIpAddress<TAddress> : IEquatable<TAddress>, ISpanParsable<TAddress>, IIpAddress
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    static abstract TAddress Any { get; }
    static abstract TAddress Local { get; }
}
