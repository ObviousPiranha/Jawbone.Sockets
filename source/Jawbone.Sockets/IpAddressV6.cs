using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets;

// https://en.wikipedia.org/wiki/IPv6_address
[StructLayout(LayoutKind.Explicit, Size = 20, Pack = 4)]
public struct IpAddressV6 : IIpAddress<IpAddressV6>
{
    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(Length)]
    public struct ArrayU8
    {
        public const int Length = 16;
        private byte _first;
    }

    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(Length)]
    public struct ArrayU16
    {
        public const int Length = 8;
        private ushort _first;
    }

    [StructLayout(LayoutKind.Sequential)]
    [InlineArray(Length)]
    public struct ArrayU32
    {
        public const int Length = 4;
        private uint _first;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalMask() => BitConverter.IsLittleEndian ? 0x0000c0ff : 0xffc00000;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint LinkLocalSubnet() => BitConverter.IsLittleEndian ? 0x000080fe : 0xfe800000;

    private static IpAddressV6 CreateLocal()
    {
        var result = default(IpAddressV6);
        result.DataU8[^1] = 1;
        return result;
    }

    public static IpAddressV6 Any => default;
    public static IpAddressV6 Local { get; } = CreateLocal();

    private static readonly uint PrefixV4 = BitConverter.IsLittleEndian ? 0xffff0000 : 0x0000ffff;

    [FieldOffset(0)]
    public ArrayU8 DataU8;

    [FieldOffset(0)]
    public ArrayU16 DataU16;

    [FieldOffset(0)]
    public ArrayU32 DataU32;

    [FieldOffset(16)]
    public uint ScopeId;

    public readonly bool IsDefault => DataU32[0] == 0 && DataU32[1] == 0 && DataU32[2] == 0 && DataU32[3] == 0;
    public readonly bool IsLinkLocal => (DataU32[0] & LinkLocalMask()) == LinkLocalSubnet();
    public readonly bool IsLoopback => Equals(Local) || (TryMapV4(out var v4) && v4.IsLoopback);
    public readonly bool IsV4Mapped => DataU32[0] == 0 && DataU32[1] == 0 && DataU32[2] == PrefixV4;

    public IpAddressV6(ReadOnlySpan<byte> values) : this(values, 0)
    {
    }

    public IpAddressV6(ReadOnlySpan<byte> values, uint scopeId) : this()
    {
        values.Slice(0, ArrayU8.Length).CopyTo(DataU8);
        ScopeId = scopeId;
    }

    public IpAddressV6(ArrayU32 data, uint scopeId = 0)
    {
        DataU32 = data;
        ScopeId = scopeId;
    }

    private IpAddressV6(uint a, uint b, uint c, uint d, uint scopeId = 0)
    {
        DataU32[0] = a;
        DataU32[1] = b;
        DataU32[2] = c;
        DataU32[3] = d;
        ScopeId = scopeId;
    }

    public readonly bool TryMapV4(out IpAddressV4 address)
    {
        if (IsV4Mapped)
        {
            address = new(DataU32[3]);
            return true;
        }
        else
        {
            address = default;
            return false;
        }
    }

    public readonly bool Equals(IpAddressV6 other)
    {
        return
            DataU32[0] == other.DataU32[0] &&
            DataU32[1] == other.DataU32[1] &&
            DataU32[2] == other.DataU32[2] &&
            DataU32[3] == other.DataU32[3] &&
            ScopeId == other.ScopeId;
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
        => obj is IpAddressV6 other && Equals(other);
    public override readonly int GetHashCode() => HashCode.Combine(DataU32[0], DataU32[1], DataU32[2], DataU32[3], ScopeId);
    public override readonly string ToString()
    {
        Span<char> buffer = stackalloc char[64];
        TryFormat(buffer, out var n, default, default);
        return buffer[..n].ToString();
    }

    public readonly bool TryFormat(
        Span<byte> utf8Destination,
        out int bytesWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var writer = SpanWriter.Create(utf8Destination);
        if (IsV4Mapped)
        {
            var success =
                writer.TryWrite("::ffff:"u8) &&
                writer.TryWriteIpAddress(new IpAddressV4(DataU32[3]));

            bytesWritten = writer.Position;
            return success;
        }

        int zeroStart = 0;
        int zeroLength = 0;

        for (int i = 0; i < ArrayU16.Length; ++i)
        {
            if (DataU16[i] != 0)
                continue;

            int j = i + 1;
            while (j < ArrayU16.Length && DataU16[j] == 0)
                ++j;

            var length = j - i;

            if (zeroLength < length)
            {
                zeroStart = i;
                zeroLength = length;
            }

            i = j;
        }

        bool result;
        if (1 < zeroLength)
        {
            result =
                writer.TryWriteV6Block(DataU16[..zeroStart]) &&
                writer.TryWrite((byte)':') &&
                writer.TryWrite((byte)':') &&
                writer.TryWriteV6Block(DataU16[(zeroStart + zeroLength)..]);
        }
        else
        {
            result = writer.TryWriteV6Block(DataU16);
        }

        if (result && ScopeId != 0)
        {
            result =
                writer.TryWrite((byte)'%') &&
                writer.TryWriteBase10(ScopeId);
        }

        bytesWritten = writer.Position;
        return result;
    }

    public readonly bool TryFormat(Span<byte> utf8Destination, out int bytesWritten) => TryFormat(utf8Destination, out bytesWritten);

    public readonly bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var writer = SpanWriter.Create(destination);
        if (IsV4Mapped)
        {
            var success =
                writer.TryWrite("::ffff:") &&
                writer.TryWriteIpAddress(new IpAddressV4(DataU32[3]));

            charsWritten = writer.Position;
            return success;
        }

        int zeroStart = 0;
        int zeroLength = 0;

        for (int i = 0; i < ArrayU16.Length; ++i)
        {
            if (DataU16[i] != 0)
                continue;

            int j = i + 1;
            while (j < ArrayU16.Length && DataU16[j] == 0)
                ++j;

            var length = j - i;

            if (zeroLength < length)
            {
                zeroStart = i;
                zeroLength = length;
            }

            i = j;
        }

        bool result;
        if (1 < zeroLength)
        {
            result =
                writer.TryWriteV6Block(DataU16[..zeroStart]) &&
                writer.TryWrite(':') &&
                writer.TryWrite(':') &&
                writer.TryWriteV6Block(DataU16[(zeroStart + zeroLength)..]);
        }
        else
        {
            result = writer.TryWriteV6Block(DataU16);
        }

        if (result && ScopeId != 0)
        {
            result =
                writer.TryWrite('%') &&
                writer.TryWriteBase10(ScopeId);
        }

        charsWritten = writer.Position;
        return result;
    }

    public readonly bool TryFormat(Span<char> destination, out int charsWritten) => TryFormat(destination, out charsWritten);

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ToString();

    private static bool DoTheParse(ReadOnlySpan<char> originalInput, bool throwException, out IpAddressV6 result)
    {
        const string BadHexBlock = "Bad hex block.";
        if (originalInput.IsEmpty)
        {
            Throw("Input string is empty.");
            result = default;
            return false;
        }

        var s = originalInput;

        {
            var hasOpeningBracket = s[0] == '[';
            var hasClosingBracket = s[^1] == ']';

            if (hasOpeningBracket != hasClosingBracket)
            {
                Throw(hasOpeningBracket ? "Missing closing bracket." : "Missing opening bracket.");
                result = default;
                return false;
            }

            if (hasOpeningBracket)
                s = s[1..^1];
        }

        if (s.Length < 2)
        {
            Throw("Input string too short.");
            result = default;
            return false;
        }

        // TODO: Make this properly flexible.
        // This would still miss plenty of other valid representations
        // for IPv4-mapped addresses, but for now, it is consistent
        // with how IpAddressV6 converts such addresses to strings.
        const string IntroV4 = "::ffff:";
        if (s.StartsWith(IntroV4) && IpAddressV4.TryParse(s[IntroV4.Length..], null, out var a32))
        {
            result = (IpAddressV6)a32;
            return true;
        }

        var scopeId = default(uint);
        {
            var scopeIndex = s.LastIndexOf('%');
            if (0 <= scopeIndex)
            {
                if (!uint.TryParse(s.Slice(scopeIndex + 1), out scopeId))
                {
                    Throw("Invalid scope ID.");
                    result = default;
                    return false;
                }

                s = s[..scopeIndex];
            }
        }

        var blocks = default(ArrayU16);
        var division = s.IndexOf("::");

        if (0 <= division)
        {
            if (!TryParseHexBlocks(s[..division], blocks, out var leftBlocksWritten))
            {
                Throw(BadHexBlock);
                result = default;
                return false;
            }

            if (!TryParseHexBlocks(s[(division + 2)..], blocks[leftBlocksWritten..], out var rightBlocksWritten))
            {
                Throw(BadHexBlock);
                result = default;
                return false;
            }

            if (leftBlocksWritten + rightBlocksWritten == ArrayU16.Length)
            {
                Throw("Malformed representation.");
                result = default;
                return false;
            }

            var end = leftBlocksWritten + rightBlocksWritten;
            blocks[leftBlocksWritten..end].CopyTo(blocks[^rightBlocksWritten..]);
            blocks[leftBlocksWritten..^rightBlocksWritten].Clear();
        }
        else if (!TryParseHexBlocks(s, blocks, out var blocksWritten) || blocksWritten < ArrayU16.Length)
        {
            Throw(BadHexBlock);
            result = default;
            return false;
        }

        result = default;
        result.DataU16 = blocks;
        result.ScopeId = scopeId;

        if (BitConverter.IsLittleEndian)
        {
            foreach (ref var n in result.DataU16)
                n = BinaryPrimitives.ReverseEndianness(n);
        }
        return true;

        void Throw(string message)
        {
            if (throwException)
                throw new FormatException(message);
        }

        static bool TryParseHexBlocks(ReadOnlySpan<char> s, Span<ushort> blocks, out int blocksWritten)
        {
            blocksWritten = 0;

            if (s.IsEmpty)
                return true;

            var index = ParseHexBlock(s, out var block);
            if (index == 0)
                return false;

            blocks[0] = block;
            blocksWritten = 1;

            while (blocksWritten < blocks.Length)
            {
                if (index == s.Length)
                    return true;

                if (s[index] != ':')
                    return false;

                var length = ParseHexBlock(s[++index..], out block);

                if (length == 0)
                    return false;

                blocks[blocksWritten++] = block;
                index += length;
            }

            return index == s.Length;
        }

        static int ParseHexBlock(ReadOnlySpan<char> s, out ushort u16)
        {
            if (s.IsEmpty)
            {
                u16 = default;
                return 0;
            }

            int result = HexDigit(s[0]);

            if (result == -1)
            {
                u16 = default;
                return 0;
            }

            int digitCount = 1;

            while (digitCount < s.Length)
            {
                var nextDigit = HexDigit(s[digitCount]);

                if (nextDigit == -1)
                    break;

                if (4 < ++digitCount)
                {
                    u16 = default;
                    return 0;
                }

                result = (result << 4) | nextDigit;
            }

            u16 = (ushort)result;
            return digitCount;
        }

        static int HexDigit(int c)
        {
            if ('0' <= c && c <= '9')
                return c - '0';
            if ('a' <= c && c <= 'f')
                return c - 'a' + 10;
            if ('A' <= c && c <= 'F')
                return c - 'A' + 10;

            return -1;
        }
    }

    public static IpAddressV6 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        DoTheParse(s, true, out var result);
        return result;
    }

    public static IpAddressV6 Parse(ReadOnlySpan<char> s) => Parse(s, null);

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out IpAddressV6 result)
    {
        return DoTheParse(s, false, out result);
    }

    public static bool TryParse(ReadOnlySpan<char> s, out IpAddressV6 result) => TryParse(s, null, out result);

    public static IpAddressV6 Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        DoTheParse(s, true, out var result);
        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out IpAddressV6 result)
    {
        return DoTheParse(s, false, out result);
    }

    public static bool operator ==(IpAddressV6 a, IpAddressV6 b) => a.Equals(b);
    public static bool operator !=(IpAddressV6 a, IpAddressV6 b) => !a.Equals(b);
    public static explicit operator IpAddressV6(IpAddressV4 address) => new(0, 0, PrefixV4, address.DataU32);
    public static explicit operator IpAddressV4(IpAddressV6 address)
    {
        if (!address.IsV4Mapped)
            throw new InvalidCastException("IPv6 address is not IPv4-mapped.");

        return new(address.DataU32[3]);
    }
}
