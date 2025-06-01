using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets;

[StructLayout(LayoutKind.Sequential)]
public struct IpEndpoint : IEquatable<IpEndpoint>, ISpanFormattable, IUtf8SpanFormattable
{
    public IpAddress Address;
    public NetworkPort Port;
    private readonly ushort _padding;

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
    IUtf8SpanFormattable
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    public TAddress Address;
    public NetworkPort Port;
    private readonly ushort _padding;
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
