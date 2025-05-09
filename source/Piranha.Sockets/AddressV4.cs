using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Piranha.Sockets;

[StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
public struct AddressV4 : IAddress<AddressV4>
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalMask() => BitConverter.IsLittleEndian ? 0x0000ffff : 0xffff0000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalSubnet() => BitConverter.IsLittleEndian ? 0x0000fea9 : 0xa9fe0000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoopbackMask() => BitConverter.IsLittleEndian ? 0x000000ff : 0xff000000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LoopbackSubnet() => BitConverter.IsLittleEndian ? 0x0000007f : (uint)0x7f000000;

    public static AddressV4 Any => default;
    public static AddressV4 Local { get; } = new(127, 0, 0, 1);
    public static AddressV4 Broadcast { get; } = new(255, 255, 255, 255);

    [FieldOffset(0)]
    public ArrayU8 DataU8;

    [FieldOffset(0)]
    public ArrayU16 DataU16;

    [FieldOffset(0)]
    public uint DataU32;

    public readonly bool IsDefault => DataU32 == 0;
    public readonly bool IsLinkLocal => (DataU32 & LinkLocalMask()) == LinkLocalSubnet();
    public readonly bool IsLoopback => (DataU32 & LoopbackMask()) == LoopbackSubnet();

    public AddressV4(ReadOnlySpan<byte> values)
    {
        DataU32 = BitConverter.ToUInt32(values);
    }

    public AddressV4(byte a, byte b, byte c, byte d)
    {
        DataU8[0] = a;
        DataU8[1] = b;
        DataU8[2] = c;
        DataU8[3] = d;
    }

    public AddressV4(uint address) => DataU32 = address;

    public readonly bool Equals(AddressV4 other) => DataU32 == other.DataU32;
    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is AddressV4 other && Equals(other);
    public override readonly int GetHashCode() => DataU32.GetHashCode();
    public override readonly string ToString()
    {
        var builder = new StringBuilder(15);
        AppendTo(builder);
        return builder.ToString();
    }

    public readonly void AppendTo(StringBuilder builder)
    {
        builder
            .Append(DataU8[0])
            .Append('.')
            .Append(DataU8[1])
            .Append('.')
            .Append(DataU8[2])
            .Append('.')
            .Append(DataU8[3]);
    }

    private static string? DoTheParse(ReadOnlySpan<char> s, out AddressV4 result)
    {
        // TODO: Support atypical formats.
        // https://en.wikipedia.org/wiki/Internet_Protocol_version_4#Address_representations

        const string UnableToParseByte = "Unable to parse byte.";
        const string MissingDot = "Missing dot.";

        var bytes = default(ArrayU8);

        if (!TryParseByte(s, out var b))
        {
            result = default;
            return UnableToParseByte;
        }

        bytes[0] = b;
        int parseIndex = Length(b);
        int next = 1;

        for (int i = 0; i < 3; ++i)
        {
            if (parseIndex == s.Length || s[parseIndex] != '.')
            {
                result = default;
                return MissingDot;
            }

            if (!TryParseByte(s[++parseIndex..], out b))
            {
                result = default;
                return UnableToParseByte;
            }

            parseIndex += Length(b);
            bytes[next++] = b;
        }

        result = new AddressV4(bytes);
        return null;

        static int Length(int b) => 100 <= b ? 3 : 10 <= b ? 2 : 1;
        static bool IsDigit(int c) => '0' <= c && c <= '9';
        static bool TryParseByte(ReadOnlySpan<char> span, out byte b)
        {
            if (span.IsEmpty || !IsDigit(span[0]))
            {
                b = default;
                return false;
            }

            int result = span[0] - '0';

            for (int i = 1; i < span.Length; ++i)
            {
                int c = span[i];

                if (!IsDigit(c))
                {
                    b = (byte)result;
                    return true;
                }

                result = result * 10 + (c - '0');
                if (byte.MaxValue < result)
                {
                    b = default;
                    return false;
                }
            }

            b = (byte)result;
            return true;
        }
    }

    public static AddressV4 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        var exceptionMessage = DoTheParse(s, out var result);
        if (exceptionMessage is not null)
            throw new FormatException(exceptionMessage);
        return result;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out AddressV4 result)
    {
        var exceptionMessage = DoTheParse(s, out result);
        return exceptionMessage is null;
    }

    public static AddressV4 Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out AddressV4 result)
    {
        return TryParse(s.AsSpan(), provider, out result);
    }

    public static Span<byte> AsBytes(ref AddressV4 address) => address.DataU8;
    public static ReadOnlySpan<byte> AsReadOnlyBytes(ref readonly AddressV4 address) => address.DataU8;

    public static bool operator ==(AddressV4 a, AddressV4 b) => a.Equals(b);
    public static bool operator !=(AddressV4 a, AddressV4 b) => !a.Equals(b);
}
