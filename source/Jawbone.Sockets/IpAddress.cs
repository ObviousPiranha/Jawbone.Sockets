using System;

namespace Jawbone.Sockets;

public readonly struct IpAddress : IEquatable<IpAddress>
{
    private readonly IpAddressV6 _storage;

    public readonly IpAddressVersion Version { get; }

    public IpAddress(IpAddressV4 address)
    {
        Version = IpAddressVersion.V4;
        _storage.DataU32[0] = address.DataU32;
    }

    public IpAddress(IpAddressV6 address)
    {
        Version = IpAddressVersion.V6;
        _storage = address;
    }

    internal readonly IpAddressV4 AsV4() => new(_storage.DataU32[0]);
    internal readonly IpAddressV6 AsV6() => _storage;

    public readonly override bool Equals(object? obj) => obj is IpAddress other && Equals(other);

    public readonly override int GetHashCode()
    {
        return Version switch
        {
            IpAddressVersion.V4 => AsV4().GetHashCode(),
            IpAddressVersion.V6 => AsV6().GetHashCode(),
            _ => 0
        };
    }

    public readonly override string? ToString()
    {
        return Version switch
        {
            IpAddressVersion.V4 => AsV4().ToString(),
            IpAddressVersion.V6 => AsV6().ToString(),
            _ => null
        };
    }

    public readonly bool Equals(IpAddress other)
    {
        return Version == other.Version && Version switch
        {
            IpAddressVersion.V4 => AsV4().Equals(other.AsV4()),
            IpAddressVersion.V6 => AsV6().Equals(other.AsV6()),
            _ => true
        };
    }

    public static explicit operator IpAddressV4(IpAddress address)
    {
        if (address.Version != IpAddressVersion.V4)
            throw new InvalidCastException();

        return address.AsV4();
    }

    public static explicit operator IpAddressV6(IpAddress address)
    {
        if (address.Version != IpAddressVersion.V6)
            throw new InvalidCastException();

        return address.AsV6();
    }

    public static implicit operator IpAddress(IpAddressV4 address) => new(address);
    public static implicit operator IpAddress(IpAddressV6 address) => new(address);
    public static bool operator ==(IpAddress a, IpAddress b) => a.Equals(b);
    public static bool operator !=(IpAddress a, IpAddress b) => !a.Equals(b);
}
