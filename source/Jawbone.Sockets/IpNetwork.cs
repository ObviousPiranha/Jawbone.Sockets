using System;
using System.Diagnostics.CodeAnalysis;

namespace Jawbone.Sockets;

public readonly struct IpNetwork :
    IEquatable<IpNetwork>,
    ISpanFormattable,
    IUtf8SpanFormattable,
    ISpanParsable<IpNetwork>,
    IUtf8SpanParsable<IpNetwork>
{
    public readonly IpAddress BaseAddress { get; }
    public readonly int PrefixLength { get; }

    internal IpNetwork(IpAddress baseAddress, int prefixLength)
    {
        BaseAddress = baseAddress;
        PrefixLength = prefixLength;
    }

    public readonly bool Equals(IpNetwork other) => BaseAddress == other.BaseAddress && PrefixLength == other.PrefixLength;
    public override readonly bool Equals([NotNullWhen(false)] object? obj) => obj is IpNetwork other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(BaseAddress, PrefixLength);
    public override readonly string ToString() => SpanWriter.GetString(this);

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        var writer = SpanWriter.Create(destination);
        var result =
            writer.TryWriteFormattable(BaseAddress, format, provider) &&
            writer.TryWrite('/') &&
            writer.TryWriteFormattable(PrefixLength);
        charsWritten = writer.Position;
        return result;
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        var writer = SpanWriter.Create(utf8Destination);
        var result =
            writer.TryWriteFormattable(BaseAddress, format, provider) &&
            writer.TryWrite((byte)'/') &&
            writer.TryWriteFormattable(PrefixLength);
        bytesWritten = writer.Position;
        return result;
    }

    public static IpNetwork Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = default)
    {
        if (!TryParse(s, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out IpNetwork result)
    {
        var slash = s.LastIndexOf('/');
        if (slash == -1 ||
            !IpAddress.TryParse(s[..slash], out var ipAddress) ||
            !int.TryParse(s.Slice(slash + 1), out var prefixLength) ||
            !IpAddress.TryCreateNetwork(ipAddress, prefixLength, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        out IpNetwork result)
    {
        return TryParse(s, default, out result);
    }

    public static IpNetwork Parse(
        string s,
        IFormatProvider? provider = default)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (!TryParse(s.AsSpan(), provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out IpNetwork result)
    {
        return TryParse(s.AsSpan(), provider, out result);
    }

    public static IpNetwork Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider = default)
    {
        if (!TryParse(utf8Text, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out IpNetwork result)
    {
        var slash = utf8Text.LastIndexOf((byte)'/');
        if (slash == -1 ||
            !IpAddress.TryParse(utf8Text[..slash], out var ipAddress) ||
            !int.TryParse(utf8Text.Slice(slash + 1), out var prefixLength) ||
            !IpAddress.TryCreateNetwork(ipAddress, prefixLength, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        out IpNetwork result)
    {
        return TryParse(utf8Text, default, out result);
    }

    public static IpNetwork<TAddress> Create<TAddress>(TAddress ipAddress, int prefixLength)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.CreateNetwork(ipAddress, prefixLength);
    }

    public static bool operator ==(IpNetwork a, IpNetwork b) => a.Equals(b);
    public static bool operator !=(IpNetwork a, IpNetwork b) => !a.Equals(b);
}

public readonly struct IpNetwork<TAddress> :
    IEquatable<IpNetwork<TAddress>>,
    ISpanFormattable,
    IUtf8SpanFormattable,
    ISpanParsable<IpNetwork<TAddress>>,
    IUtf8SpanParsable<IpNetwork<TAddress>>
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    public readonly TAddress BaseAddress { get; }
    public readonly int PrefixLength { get; }
    public TAddress MaxAddress => TAddress.GetMaxAddress(this);

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
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
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

    public static IpNetwork<TAddress> Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = default)
    {
        if (!TryParse(s, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out IpNetwork<TAddress> result)
    {
        var slash = s.LastIndexOf('/');
        if (slash == -1 ||
            !TAddress.TryParse(s[..slash], out var ipAddress) ||
            !int.TryParse(s.Slice(slash + 1), out var prefixLength) ||
            !TAddress.TryCreateNetwork(ipAddress, prefixLength, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        out IpNetwork<TAddress> result)
    {
        return TryParse(s, default, out result);
    }

    public static IpNetwork<TAddress> Parse(
        string s,
        IFormatProvider? provider = default)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out IpNetwork<TAddress> result)
    {
        return TryParse(s.AsSpan(), provider, out result);
    }

    public static IpNetwork<TAddress> Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider = default)
    {
        if (!TryParse(utf8Text, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out IpNetwork<TAddress> result)
    {
        var slash = utf8Text.LastIndexOf((byte)'/');
        if (slash == -1 ||
            !TAddress.TryParse(utf8Text[..slash], out var ipAddress) ||
            !int.TryParse(utf8Text.Slice(slash + 1), out var prefixLength) ||
            !TAddress.TryCreateNetwork(ipAddress, prefixLength, out result))
        {
            result = default;
            return false;
        }

        return true;
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        out IpNetwork<TAddress> result)
    {
        return TryParse(utf8Text, default, out result);
    }

    public static IpNetwork<TAddress> LinkLocal => TAddress.LinkLocalNetwork;

    public static implicit operator IpNetwork(IpNetwork<TAddress> ipNetwork) => new(ipNetwork.BaseAddress, ipNetwork.PrefixLength);
    public static explicit operator IpNetwork<TAddress>(IpNetwork ipNetwork) => new((TAddress)ipNetwork.BaseAddress, ipNetwork.PrefixLength);

    public static bool operator ==(IpNetwork<TAddress> a, IpNetwork<TAddress> b) => a.Equals(b);
    public static bool operator !=(IpNetwork<TAddress> a, IpNetwork<TAddress> b) => !a.Equals(b);
}
