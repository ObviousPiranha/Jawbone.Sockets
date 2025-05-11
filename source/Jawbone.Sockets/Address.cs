using System;

namespace Jawbone.Sockets;

public readonly struct Address : IEquatable<Address>
{
    private readonly AddressV6 _storage;

    public readonly AddressType Type { get; }

    public Address(AddressV4 address)
    {
        Type = AddressType.V4;
        _storage.DataU32[0] = address.DataU32;
    }

    public Address(AddressV6 address)
    {
        Type = AddressType.V6;
        _storage = address;
    }

    public readonly AddressV4 AsV4() => new(_storage.DataU32[0]);
    public readonly AddressV6 AsV6() => _storage;

    public readonly override bool Equals(object? obj) => obj is Address other && Equals(other);

    public readonly override int GetHashCode()
    {
        return Type switch
        {
            AddressType.V4 => AsV4().GetHashCode(),
            AddressType.V6 => AsV6().GetHashCode(),
            _ => 0
        };
    }

    public readonly override string? ToString()
    {
        return Type switch
        {
            AddressType.V4 => AsV4().ToString(),
            AddressType.V6 => AsV6().ToString(),
            _ => null
        };
    }

    public readonly bool Equals(Address other)
    {
        return Type == other.Type && Type switch
        {
            AddressType.V4 => AsV4().Equals(other.AsV4()),
            AddressType.V6 => AsV6().Equals(other.AsV6()),
            _ => true
        };
    }

    public static explicit operator AddressV4(Address address)
    {
        if (address.Type != AddressType.V4)
            throw new InvalidCastException();

        return address.AsV4();
    }

    public static explicit operator AddressV6(Address address)
    {
        if (address.Type != AddressType.V6)
            throw new InvalidCastException();

        return address.AsV6();
    }

    public static implicit operator Address(AddressV4 address) => new(address);
    public static implicit operator Address(AddressV6 address) => new(address);
    public static bool operator ==(Address a, Address b) => a.Equals(b);
    public static bool operator !=(Address a, Address b) => !a.Equals(b);
}
