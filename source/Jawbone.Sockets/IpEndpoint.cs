using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jawbone.Sockets;

public struct IpEndpoint : IEquatable<IpEndpoint>
{
    public IpAddress Address;
    public NetworkPort Port;

    public IpEndpoint(IpAddress address, NetworkPort port)
    {
        Address = address;
        Port = port;
    }

    public IpEndpoint(IpAddress address, int port) : this(address, (NetworkPort)port)
    {
    }

    public readonly bool Equals(IpEndpoint other) => Address.Equals(other.Address) && Port.Equals(other.Port);
    public readonly override bool Equals(object? obj) => obj is IpEndpoint other && Equals(other);
    public readonly override int GetHashCode() => HashCode.Combine(Address, Port);

    public readonly override string ToString() => $"[{Address}]:{Port}";

    internal readonly IpEndpoint<IpAddressV4> AsV4() => Address.AsV4().OnPort(Port);
    internal readonly IpEndpoint<IpAddressV6> AsV6() => Address.AsV6().OnPort(Port);

    public static bool operator ==(IpEndpoint a, IpEndpoint b) => a.Equals(b);
    public static bool operator !=(IpEndpoint a, IpEndpoint b) => !a.Equals(b);
    public static implicit operator IpEndpoint(IpEndpoint<IpAddressV4> endpoint) => new(endpoint.Address, endpoint.Port);
    public static implicit operator IpEndpoint(IpEndpoint<IpAddressV6> endpoint) => new(endpoint.Address, endpoint.Port);
    public static explicit operator IpEndpoint<IpAddressV4>(IpEndpoint endpoint) => new((IpAddressV4)endpoint.Address, endpoint.Port);
    public static explicit operator IpEndpoint<IpAddressV6>(IpEndpoint endpoint) => new((IpAddressV6)endpoint.Address, endpoint.Port);

    public static IpEndpoint Create(IpAddress address, int port) => new(address, port);
    public static IpEndpoint Create(IpAddress address, NetworkPort port) => new(address, port);

    public static IpEndpoint<TAddress> Create<TAddress>(TAddress address, int port)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return new(address, port);
    }

    public static IpEndpoint<TAddress> Create<TAddress>(TAddress address, NetworkPort port)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return new(address, port);
    }

    public static IpEndpoint<IpAddressV4> ConvertToV4(IpEndpoint<IpAddressV6> endpoint)
    {
        return Create((IpAddressV4)endpoint.Address, endpoint.Port);
    }

    public static IpEndpoint<IpAddressV6> ConvertToV6(IpEndpoint<IpAddressV4> endpoint)
    {
        return Create((IpAddressV6)endpoint.Address, endpoint.Port);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct IpEndpoint<TAddress> : IEquatable<IpEndpoint<TAddress>>
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    public TAddress Address;
    public NetworkPort Port;
    public readonly bool IsDefault => Address.IsDefault && Port.NetworkValue == 0;

    public IpEndpoint(TAddress address, NetworkPort port)
    {
        Address = address;
        Port = port;
    }

    public IpEndpoint(TAddress address, int port) : this(address, (NetworkPort)port)
    {
    }

    public readonly bool Equals(IpEndpoint<TAddress> other) => Address.Equals(other.Address) && Port.Equals(other.Port);
    public override readonly bool Equals(object? obj) => obj is IpEndpoint<TAddress> other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(Address, Port);
    public override readonly string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendEndpoint(this);
        return builder.ToString();
    }

    public static bool operator ==(IpEndpoint<TAddress> a, IpEndpoint<TAddress> b) => a.Equals(b);
    public static bool operator !=(IpEndpoint<TAddress> a, IpEndpoint<TAddress> b) => !a.Equals(b);
}
