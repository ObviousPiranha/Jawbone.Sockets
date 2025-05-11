using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Jawbone.Sockets;

public struct Endpoint : IEquatable<Endpoint>
{
    public Address Address;
    public NetworkPort Port;

    public Endpoint(Address address, NetworkPort port)
    {
        Address = address;
        Port = port;
    }

    public Endpoint(Address address, int port) : this(address, (NetworkPort)port)
    {
    }

    public readonly bool Equals(Endpoint other) => Address.Equals(other.Address) && Port.Equals(other.Port);
    public readonly override bool Equals(object? obj) => obj is Endpoint other && Equals(other);
    public readonly override int GetHashCode() => HashCode.Combine(Address, Port);

    public static bool operator ==(Endpoint a, Endpoint b) => a.Equals(b);
    public static bool operator !=(Endpoint a, Endpoint b) => !a.Equals(b);
    public static implicit operator Endpoint(Endpoint<AddressV4> endpoint) => new(endpoint.Address, endpoint.Port);
    public static implicit operator Endpoint(Endpoint<AddressV6> endpoint) => new(endpoint.Address, endpoint.Port);
    public static explicit operator Endpoint<AddressV4>(Endpoint endpoint) => new((AddressV4)endpoint.Address, endpoint.Port);
    public static explicit operator Endpoint<AddressV6>(Endpoint endpoint) => new((AddressV6)endpoint.Address, endpoint.Port);

    public static Endpoint Create(Address address, int port) => new(address, port);
    public static Endpoint Create(Address address, NetworkPort port) => new(address, port);

    public static Endpoint<TAddress> Create<TAddress>(TAddress address, int port)
        where TAddress : unmanaged, IAddress<TAddress>
    {
        return new(address, port);
    }

    public static Endpoint<TAddress> Create<TAddress>(TAddress address, NetworkPort port)
        where TAddress : unmanaged, IAddress<TAddress>
    {
        return new(address, port);
    }

    public static Endpoint<AddressV4> ConvertToV4(Endpoint<AddressV6> endpoint)
    {
        return Create((AddressV4)endpoint.Address, endpoint.Port);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct Endpoint<TAddress> : IEquatable<Endpoint<TAddress>>
    where TAddress : unmanaged, IAddress<TAddress>
{
    public TAddress Address;
    public NetworkPort Port;
    public readonly bool IsDefault => Address.IsDefault && Port.NetworkValue == 0;

    public Endpoint(TAddress address, NetworkPort port)
    {
        Address = address;
        Port = port;
    }

    public Endpoint(TAddress address, int port) : this(address, (NetworkPort)port)
    {
    }

    public readonly bool Equals(Endpoint<TAddress> other) => Address.Equals(other.Address) && Port.Equals(other.Port);
    public override readonly bool Equals(object? obj) => obj is Endpoint<TAddress> other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(Address, Port);
    public override readonly string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendEndpoint(this);
        return builder.ToString();
    }

    public static bool operator ==(Endpoint<TAddress> a, Endpoint<TAddress> b) => a.Equals(b);
    public static bool operator !=(Endpoint<TAddress> a, Endpoint<TAddress> b) => !a.Equals(b);
}
