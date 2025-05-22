using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets;

[StructLayout(LayoutKind.Sequential)]
public struct NetworkPort :
    IEquatable<NetworkPort>,
    IParsable<NetworkPort>,
    ISpanParsable<NetworkPort>,
    IUtf8SpanParsable<NetworkPort>,
    ISpanFormattable,
    IUtf8SpanFormattable
{
    public ushort NetworkValue;

    public readonly ushort HostValue
    {
        get
        {
            return BitConverter.IsLittleEndian ?
                BinaryPrimitives.ReverseEndianness(NetworkValue) :
                NetworkValue;
        }

        init
        {
            NetworkValue = BitConverter.IsLittleEndian ?
                BinaryPrimitives.ReverseEndianness(value) :
                value;
        }
    }

    public NetworkPort(int port) => HostValue = checked((ushort)port);
    public readonly bool Equals(NetworkPort other) => NetworkValue == other.NetworkValue;
    public readonly override bool Equals([NotNullWhen(true)] object? obj) => obj is NetworkPort other && Equals(other);
    public readonly override int GetHashCode() => NetworkValue.GetHashCode();
    public readonly override string ToString() => HostValue.ToString();

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        return HostValue.TryFormat(
            utf8Destination,
            out bytesWritten,
            format,
            provider);
    }

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default)
    {
        return HostValue.TryFormat(
            destination,
            out charsWritten,
            format,
            provider);
    }

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    public static NetworkPort Parse(
        string s,
        IFormatProvider? provider = default)
    {
        return new NetworkPort { HostValue = ushort.Parse(s) };
    }

    public static bool TryParse(
        [NotNullWhen(true)] string? s,
        IFormatProvider? provider,
        out NetworkPort result)
    {
        if (ushort.TryParse(s, provider, out var hostValue))
        {
            result = new NetworkPort { HostValue = hostValue };
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static NetworkPort Parse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider = default)
    {
        return new NetworkPort { HostValue = ushort.Parse(s) };
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        out NetworkPort result)
    {
        return TryParse(s, default, out result);
    }

    public static bool TryParse(
        ReadOnlySpan<char> s,
        IFormatProvider? provider,
        out NetworkPort result)
    {
        if (ushort.TryParse(s, provider, out var hostValue))
        {
            result = new NetworkPort { HostValue = hostValue };
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static NetworkPort Parse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider = default)
    {
        return new NetworkPort { HostValue = ushort.Parse(utf8Text) };
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        out NetworkPort result)
    {
        return TryParse(utf8Text, default, out result);
    }

    public static bool TryParse(
        ReadOnlySpan<byte> utf8Text,
        IFormatProvider? provider,
        out NetworkPort result)
    {
        if (ushort.TryParse(utf8Text, provider, out var hostValue))
        {
            result = new NetworkPort { HostValue = hostValue };
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static explicit operator NetworkPort(int port) => new(port);
    public static bool operator ==(NetworkPort a, NetworkPort b) => a.Equals(b);
    public static bool operator !=(NetworkPort a, NetworkPort b) => !a.Equals(b);
}
