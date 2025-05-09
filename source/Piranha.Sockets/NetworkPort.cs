using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Piranha.Sockets;

[StructLayout(LayoutKind.Sequential)]
public readonly struct NetworkPort : IEquatable<NetworkPort>, IParsable<NetworkPort>, ISpanParsable<NetworkPort>
{
    public readonly ushort NetworkValue { get; init; }

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

    public static NetworkPort Parse(string s, IFormatProvider? provider)
    {
        return new NetworkPort { HostValue = ushort.Parse(s) };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out NetworkPort result)
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

    public static NetworkPort Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return new NetworkPort { HostValue = ushort.Parse(s) };
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out NetworkPort result)
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

    public static explicit operator NetworkPort(int port) => new(port);
    public static bool operator ==(NetworkPort a, NetworkPort b) => a.Equals(b);
    public static bool operator !=(NetworkPort a, NetworkPort b) => !a.Equals(b);
}
