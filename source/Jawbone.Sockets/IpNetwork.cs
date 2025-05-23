using System;
using System.Diagnostics.CodeAnalysis;

namespace Jawbone.Sockets;

public readonly struct IpNetwork<TAddress> :
    IEquatable<IpNetwork<TAddress>>,
    ISpanFormattable,
    IUtf8SpanFormattable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    public readonly TAddress BaseAddress { get; }
    public readonly int PrefixLength { get; }

    internal IpNetwork(TAddress baseAddress, int prefixLength)
    {
        BaseAddress = baseAddress;
        PrefixLength = prefixLength;
    }

    public readonly bool Equals(IpNetwork<TAddress> other) => BaseAddress.Equals(other.BaseAddress) && PrefixLength == other.PrefixLength;
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is IpNetwork<TAddress> other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(BaseAddress, PrefixLength);
    public override readonly string ToString() => SpanWriter.GetString(this);

    public readonly bool Contains(TAddress address) => address.IsInNetwork(this);

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var writer = SpanWriter.Create(utf8Destination);
        var result =
            writer.TryWriteFormattable(BaseAddress) &&
            writer.TryWrite((byte)'/') &&
            writer.TryWriteFormattable(PrefixLength);
        bytesWritten = writer.Position;
        return result;
    }

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        var writer = SpanWriter.Create(destination);
        var result =
            writer.TryWriteFormattable(BaseAddress) &&
            writer.TryWrite('/') &&
            writer.TryWriteFormattable(PrefixLength);
        charsWritten = writer.Position;
        return result;
    }

    public string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public static bool operator ==(IpNetwork<TAddress> a, IpNetwork<TAddress> b) => a.Equals(b);
    public static bool operator !=(IpNetwork<TAddress> a, IpNetwork<TAddress> b) => !a.Equals(b);
}

public static class IpNetwork
{
    public static IpNetwork<TAddress> Create<TAddress>(TAddress address, int prefixLength)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.CreateNetwork(address, prefixLength);
    }
}
