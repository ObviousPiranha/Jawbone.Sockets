using System;

namespace Jawbone.Sockets;

public readonly struct IpAddress : IEquatable<IpAddress>
{
    private readonly IpAddressV6 _storage;

    public readonly IpAddressType Type { get; }

    public IpAddress(IpAddressV4 address)
    {
        Type = IpAddressType.V4;
        _storage.DataU32[0] = address.DataU32;
    }

    public IpAddress(IpAddressV6 address)
    {
        Type = IpAddressType.V6;
        _storage = address;
    }

    public readonly IpAddressV4 AsV4() => new(_storage.DataU32[0]);
    public readonly IpAddressV6 AsV6() => _storage;

    public readonly override bool Equals(object? obj) => obj is IpAddress other && Equals(other);

    public readonly override int GetHashCode()
    {
        return Type switch
        {
            IpAddressType.V4 => AsV4().GetHashCode(),
            IpAddressType.V6 => AsV6().GetHashCode(),
            _ => 0
        };
    }

    public readonly override string? ToString()
    {
        return Type switch
        {
            IpAddressType.V4 => AsV4().ToString(),
            IpAddressType.V6 => AsV6().ToString(),
            _ => null
        };
    }

    public readonly bool Equals(IpAddress other)
    {
        return Type == other.Type && Type switch
        {
            IpAddressType.V4 => AsV4().Equals(other.AsV4()),
            IpAddressType.V6 => AsV6().Equals(other.AsV6()),
            _ => true
        };
    }

    public static explicit operator IpAddressV4(IpAddress address)
    {
        if (address.Type != IpAddressType.V4)
            throw new InvalidCastException();

        return address.AsV4();
    }

    public static explicit operator IpAddressV6(IpAddress address)
    {
        if (address.Type != IpAddressType.V6)
            throw new InvalidCastException();

        return address.AsV6();
    }

    public static implicit operator IpAddress(IpAddressV4 address) => new(address);
    public static implicit operator IpAddress(IpAddressV6 address) => new(address);
    public static bool operator ==(IpAddress a, IpAddress b) => a.Equals(b);
    public static bool operator !=(IpAddress a, IpAddress b) => !a.Equals(b);
}
