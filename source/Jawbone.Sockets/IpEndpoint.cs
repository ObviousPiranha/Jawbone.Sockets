using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets;

[StructLayout(LayoutKind.Sequential)]
public struct IpEndpoint : IEquatable<IpEndpoint>, ISpanFormattable, IUtf8SpanFormattable
{
    public IpAddress Address;
    public NetworkPort Port;
    private readonly ushort _padding;

    public readonly IpAddressVersion Version => Address.Version;

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
    public override readonly string ToString() => SpanWriter.GetString(this);

    internal readonly IpEndpoint<IpAddressV4> AsV4() => Address.AsV4().OnPort(Port);
    internal readonly IpEndpoint<IpAddressV6> AsV6() => Address.AsV6().OnPort(Port);

    public static bool operator ==(IpEndpoint a, IpEndpoint b) => a.Equals(b);
    public static bool operator !=(IpEndpoint a, IpEndpoint b) => !a.Equals(b);
    public static implicit operator IpEndpoint(IPEndPoint? ipEndpoint)
    {
        if (ipEndpoint is null)
            return default;
        var result = new IpEndpoint(ipEndpoint.Address, ipEndpoint.Port);
        return result;
    }

    public static implicit operator IPEndPoint?(IpEndpoint ipEndpoint)
    {
        var ipAddress = (IPAddress?)ipEndpoint.Address;
        if (ipAddress is null)
            return default;
        var result = new IPEndPoint(ipAddress, ipEndpoint.Port.HostValue);
        return result;
    }
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

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        if (Address.Version == IpAddressVersion.V4)
        {
            return Address.AsV4().OnPort(Port).TryFormat(utf8Destination, out bytesWritten, format, provider);
        }
        else if (Address.Version == IpAddressVersion.V6)
        {
            return Address.AsV6().OnPort(Port).TryFormat(utf8Destination, out bytesWritten, format, provider);
        }
        else
        {
            var writer = SpanWriter.Create(utf8Destination);
            var result =
                writer.TryWrite((byte)':') &&
                writer.TryWriteFormattable(Port.HostValue);
            bytesWritten = writer.Position;
            return result;
        }
    }

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        if (Address.Version == IpAddressVersion.V4)
        {
            return Address.AsV4().OnPort(Port).TryFormat(destination, out charsWritten, format, provider);
        }
        else if (Address.Version == IpAddressVersion.V6)
        {
            return Address.AsV6().OnPort(Port).TryFormat(destination, out charsWritten, format, provider);
        }
        else
        {
            var writer = SpanWriter.Create(destination);
            var result =
                writer.TryWrite(':') &&
                writer.TryWriteFormattable(Port.HostValue);
            charsWritten = writer.Position;
            return result;
        }
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();
}

[StructLayout(LayoutKind.Sequential)]
public struct IpEndpoint<TAddress> :
    IEquatable<IpEndpoint<TAddress>>,
    ISpanFormattable,
    IUtf8SpanFormattable,
    ISpanParsable<IpEndpoint<TAddress>>,
    IUtf8SpanParsable<IpEndpoint<TAddress>>
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    public TAddress Address;
    public NetworkPort Port;
    private ushort _padding;
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
    public override readonly string ToString() => SpanWriter.GetString(this);

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        var writer = SpanWriter.Create(utf8Destination);
        var result =
            writer.TryWriteFormattable(Address, default, IpAddressV6.FormatProvider.Instance) &&
            writer.TryWrite((byte)':') &&
            writer.TryWriteFormattable(Port.HostValue);
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
            writer.TryWriteFormattable(Address, default, IpAddressV6.FormatProvider.Instance) &&
            writer.TryWrite(':') &&
            writer.TryWriteFormattable(Port.HostValue);
        charsWritten = writer.Position;
        return result;
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public static IpEndpoint<TAddress> Parse(
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
        out IpEndpoint<TAddress> result)
    {
        var colon = s.LastIndexOf(':');
        if (colon == -1)
        {
            result = default;
            return false;
        }

        Unsafe.SkipInit(out result);
        if (!TAddress.TryParse(s[..colon], IpAddressV6.FormatProvider.Instance, out result.Address) ||
            !NetworkPort.TryParse(s[(colon + 1)..], out result.Port))
        {
            result = default;
            return false;
        }

        result._padding = 0;
        return true;
    }

    public static bool TryParse(ReadOnlySpan<char> s, out IpEndpoint<TAddress> result) => TryParse(s, default, out result);

    public static IpEndpoint<TAddress> Parse(string s, IFormatProvider? provider = default)
    {
        ArgumentNullException.ThrowIfNull(s);
        if (!TryParse(s.AsSpan(), provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out IpEndpoint<TAddress> result)
    {
        return TryParse(s.AsSpan(), provider, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, out IpEndpoint<TAddress> result) => TryParse(s, default, out result);

    public static IpEndpoint<TAddress> Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider = default)
    {
        if (!TryParse(utf8Text, provider, out var result))
            throw new FormatException();
        return result;
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out IpEndpoint<TAddress> result)
    {
        var colon = utf8Text.LastIndexOf((byte)':');
        if (colon == -1)
        {
            result = default;
            return false;
        }

        Unsafe.SkipInit(out result);
        if (!TAddress.TryParse(utf8Text[..colon], IpAddressV6.FormatProvider.Instance, out result.Address) ||
            !NetworkPort.TryParse(utf8Text[(colon + 1)..], out result.Port))
        {
            result = default;
            return false;
        }

        result._padding = 0;
        return true;
    }

    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out IpEndpoint<TAddress> result) => TryParse(utf8Text, default, out result);

    public static explicit operator IpEndpoint<TAddress>(IPEndPoint ipEndpoint)
    {
        ArgumentNullException.ThrowIfNull(ipEndpoint);
        var result = new IpEndpoint<TAddress>(
            (TAddress)ipEndpoint.Address,
            ipEndpoint.Port);
        return result;
    }

    public static explicit operator IPEndPoint(IpEndpoint<TAddress> ipEndpoint)
    {
        var result = new IPEndPoint(
            (IPAddress)ipEndpoint.Address,
            ipEndpoint.Port.HostValue);
        return result;
    }

    public static bool operator ==(IpEndpoint<TAddress> a, IpEndpoint<TAddress> b) => a.Equals(b);
    public static bool operator !=(IpEndpoint<TAddress> a, IpEndpoint<TAddress> b) => !a.Equals(b);
}
