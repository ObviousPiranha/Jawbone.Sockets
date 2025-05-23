using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets;

[StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
public struct IpAddressV4 : IIpAddress<IpAddressV4>
{
#pragma warning disable IDE0044
    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(Length)]
    public struct ArrayU8
    {
        public const int Length = 4;
        private byte _first;
    }

    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(Length)]
    public struct ArrayU16
    {
        public const int Length = 2;
        private ushort _first;
    }
#pragma warning restore IDE0044

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalMask() => BitConverter.IsLittleEndian ? 0x0000ffff : 0xffff0000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalSubnet() => BitConverter.IsLittleEndian ? 0x0000fea9 : 0xa9fe0000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoopbackMask() => BitConverter.IsLittleEndian ? 0x000000ff : 0xff000000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoopbackSubnet() => BitConverter.IsLittleEndian ? 0x0000007f : (uint)0x7f000000;

    public static IpAddressV4 Any => default;
    public static IpAddressV4 Local { get; } = new(127, 0, 0, 1);
    public static IpAddressV4 Broadcast { get; } = new(255, 255, 255, 255);
    public static IpAddressVersion Version => IpAddressVersion.V4;

    [FieldOffset(0)]
    public ArrayU8 DataU8;

    [FieldOffset(0)]
    public ArrayU16 DataU16;

    [FieldOffset(0)]
    public uint DataU32;

    public readonly bool IsDefault => DataU32 == 0;
    public readonly bool IsLinkLocal => (DataU32 & LinkLocalMask()) == LinkLocalSubnet();
    public readonly bool IsLoopback => (DataU32 & LoopbackMask()) == LoopbackSubnet();

    public IpAddressV4(ReadOnlySpan<byte> values)
    {
        DataU32 = BitConverter.ToUInt32(values);
    }

    public IpAddressV4(byte a, byte b, byte c, byte d)
    {
        DataU8[0] = a;
        DataU8[1] = b;
        DataU8[2] = c;
        DataU8[3] = d;
    }

    public IpAddressV4(uint address) => DataU32 = address;

    public readonly bool IsInNetwork(IpNetwork<IpAddressV4> ipNetwork)
    {
        if (ipNetwork.PrefixLength == 0)
            return true;
        var mask = uint.MaxValue << (32 - ipNetwork.PrefixLength);
        if (BitConverter.IsLittleEndian)
            mask = BinaryPrimitives.ReverseEndianness(mask);
        var result = (DataU32 & mask) == ipNetwork.BaseAddress.DataU32;
        return result;
    }

    public readonly bool Equals(IpAddressV4 other) => DataU32 == other.DataU32;
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is IpAddressV4 other && Equals(other);
    public override readonly int GetHashCode() => DataU32.GetHashCode();
    public override readonly string ToString() => SpanWriter.GetString(this);

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var writer = SpanWriter.Create(utf8Destination);
        var result =
            writer.TryWriteFormattable(DataU8[0]) &&
            writer.TryWrite((byte)'.') &&
            writer.TryWriteFormattable(DataU8[1]) &&
            writer.TryWrite((byte)'.') &&
            writer.TryWriteFormattable(DataU8[2]) &&
            writer.TryWrite((byte)'.') &&
            writer.TryWriteFormattable(DataU8[3]);
        bytesWritten = writer.Position;
        return result;
    }

    public readonly bool TryFormat(Span<byte> utf8Destination, out int bytesWritten) => TryFormat(utf8Destination, out bytesWritten, default, default);

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var writer = SpanWriter.Create(destination);
        var result =
            writer.TryWriteFormattable(DataU8[0]) &&
            writer.TryWrite('.') &&
            writer.TryWriteFormattable(DataU8[1]) &&
            writer.TryWrite('.') &&
            writer.TryWriteFormattable(DataU8[2]) &&
            writer.TryWrite('.') &&
            writer.TryWriteFormattable(DataU8[3]);
        charsWritten = writer.Position;
        return result;
    }

    public readonly bool TryFormat(Span<char> destination, out int charsWritten) => TryFormat(destination, out charsWritten, default, default);

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    private static bool DoTheParse(
        ReadOnlySpan<char> s,
        bool throwException,
        out IpAddressV4 result)
    {
        // TODO: Support atypical formats.
        // https://en.wikipedia.org/wiki/Internet_Protocol_version_4#Address_representations

        const string UnableToParseByte = "Unable to parse byte.";
        const string MissingDot = "Missing dot.";

        var bytes = default(ArrayU8);
        var parseIndex = ParseByte(s, out var b);

        if (parseIndex == 0)
        {
            Throw(UnableToParseByte);
            result = default;
            return false;
        }

        bytes[0] = b;
        int next = 1;

        for (int i = 0; i < 3; ++i)
        {
            if (parseIndex == s.Length || s[parseIndex] != '.')
            {
                Throw(MissingDot);
                result = default;
                return false;
            }

            var n = ParseByte(s[++parseIndex..], out b);
            if (n == 0)
            {
                Throw(UnableToParseByte);
                result = default;
                return false;
            }

            parseIndex += n;
            bytes[next++] = b;
        }

        result = new IpAddressV4(bytes);
        return true;

        void Throw(string message)
        {
            if (throwException)
                throw new FormatException(message);
        }
        
        static int ParseByte(ReadOnlySpan<char> span, out byte b)
        {
            if (span.IsEmpty || !IsDigit(span[0]))
            {
                b = default;
                return 0;
            }

            int result = span[0] - '0';

            for (int i = 1; i < span.Length; ++i)
            {
                int c = span[i];

                if (!IsDigit(c))
                {
                    b = (byte)result;
                    return i;
                }

                result = result * 10 + (c - '0');
                if (byte.MaxValue < result)
                {
                    b = default;
                    return 0;
                }
            }

            b = (byte)result;
            return span.Length;
        }
    }

    public static IpAddressV4 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        _ = DoTheParse(s, true, out var result);
        return result;
    }

    public static IpAddressV4 Parse(ReadOnlySpan<char> s) => Parse(s, null);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out IpAddressV4 result)
    {
        var succeeded = DoTheParse(s, false, out result);
        return succeeded;
    }

    public static bool TryParse(ReadOnlySpan<char> s, out IpAddressV4 result) => TryParse(s, null, out result);

    public static IpAddressV4 Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out IpAddressV4 result)
    {
        return TryParse(s.AsSpan(), provider, out result);
    }

    private static bool DoTheParse(
        ReadOnlySpan<byte> s,
        bool throwException,
        out IpAddressV4 result)
    {
        // TODO: Support atypical formats.
        // https://en.wikipedia.org/wiki/Internet_Protocol_version_4#Address_representations

        const string UnableToParseByte = "Unable to parse byte.";
        const string MissingDot = "Missing dot.";

        var bytes = default(ArrayU8);
        var parseIndex = ParseByte(s, out var b);

        if (parseIndex == 0)
        {
            Throw(UnableToParseByte);
            result = default;
            return false;
        }

        bytes[0] = b;
        int next = 1;

        for (int i = 0; i < 3; ++i)
        {
            if (parseIndex == s.Length || s[parseIndex] != '.')
            {
                Throw(MissingDot);
                result = default;
                return false;
            }

            var n = ParseByte(s[++parseIndex..], out b);
            if (n == 0)
            {
                Throw(UnableToParseByte);
                result = default;
                return false;
            }

            parseIndex += n;
            bytes[next++] = b;
        }

        result = new IpAddressV4(bytes);
        return true;

        void Throw(string message)
        {
            if (throwException)
                throw new FormatException(message);
        }
        
        static int ParseByte(ReadOnlySpan<byte> span, out byte b)
        {
            if (span.IsEmpty || !IsDigit(span[0]))
            {
                b = default;
                return 0;
            }

            int result = span[0] - '0';

            for (int i = 1; i < span.Length; ++i)
            {
                int c = span[i];

                if (!IsDigit(c))
                {
                    b = (byte)result;
                    return i;
                }

                result = result * 10 + (c - '0');
                if (byte.MaxValue < result)
                {
                    b = default;
                    return 0;
                }
            }

            b = (byte)result;
            return span.Length;
        }
    }

    public static IpAddressV4 Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
    {
        _ = DoTheParse(utf8Text, true, out var result);
        return result;
    }

    public static IpAddressV4 Parse(ReadOnlySpan<byte> utf8Text) => Parse(utf8Text, null);

    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out IpAddressV4 result)
    {
        var succeeded = DoTheParse(utf8Text, false, out result);
        return succeeded;
    }

    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out IpAddressV4 result) => TryParse(utf8Text, null, out result);

    public static IpNetwork<IpAddressV4> CreateNetwork(IpAddressV4 address, int prefixLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(prefixLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(prefixLength, 32);
        var mask = (uint)((long)uint.MaxValue << (32 - prefixLength));
        if (BitConverter.IsLittleEndian)
            mask = BinaryPrimitives.ReverseEndianness(mask);
        if ((address.DataU32 & mask) != address.DataU32)
            ThrowExceptionFor.InvalidNetwork(address, prefixLength);
        return new(address, prefixLength);
    }

    private static bool IsDigit(int c) => '0' <= c && c <= '9';

    public static bool operator ==(IpAddressV4 a, IpAddressV4 b) => a.Equals(b);
    public static bool operator !=(IpAddressV4 a, IpAddressV4 b) => !a.Equals(b);
}
