using System;

namespace Jawbone.Sockets;

static class Hex
{
    public const string Digits = "0123456789abcdef";
    public static ReadOnlySpan<byte> Digits8 => "0123456789abcdef"u8;

    public static bool TryWriteFullHex8(ref this SpanWriter<char> writer, int value)
    {
        var result =
            writer.TryWrite(Digits[(value >> 4) & 0xf]) &&
            writer.TryWrite(Digits[value & 0xf]);
        return result;
    }

    public static bool TryWriteFullHex8(ref this SpanWriter<byte> writer, int value)
    {
        var result =
            writer.TryWrite(Digits8[(value >> 4) & 0xf]) &&
            writer.TryWrite(Digits8[value & 0xf]);
        return result;
    }

    public static bool TryWriteHex8(ref this SpanWriter<char> writer, int value)
    {
        var hi = (value >> 4) & 0xf;
        if (0 < hi && !writer.TryWrite(Digits[hi]))
            return false;
        return writer.TryWrite(Digits[value & 0xf]);
    }

    public static bool TryWriteHex8(ref this SpanWriter<byte> writer, int value)
    {
        var hi = (value >> 4) & 0xf;
        if (0 < hi && !writer.TryWrite(Digits8[hi]))
            return false;
        return writer.TryWrite(Digits8[value & 0xf]);
    }

    public static bool TryWriteHex16(ref this SpanWriter<char> writer, int hi, int lo)
    {
        if (0 < hi)
        {
            return
                writer.TryWriteHex8(hi) &&
                writer.TryWriteFullHex8(lo);
        }
        else
        {
            return writer.TryWriteHex8(lo);
        }
    }

    public static bool TryWriteHex16(ref this SpanWriter<byte> writer, int hi, int lo)
    {
        if (0 < hi)
        {
            return
                writer.TryWriteHex8(hi) &&
                writer.TryWriteFullHex8(lo);
        }
        else
        {
            return writer.TryWriteHex8(lo);
        }
    }

    public static bool TryWriteHex16(ref this SpanWriter<char> writer, int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var hi = value & 0xff;
            var lo = (value >> 8) & 0xff;
            return writer.TryWriteHex16(hi, lo);
        }
        else
        {
            var hi = (value >> 8) & 0xff;
            var lo = value & 0xff;
            return writer.TryWriteHex16(hi, lo);
        }
    }

    public static bool TryWriteHex16(ref this SpanWriter<byte> writer, int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var hi = value & 0xff;
            var lo = (value >> 8) & 0xff;
            return writer.TryWriteHex16(hi, lo);
        }
        else
        {
            var hi = (value >> 8) & 0xff;
            var lo = value & 0xff;
            return writer.TryWriteHex16(hi, lo);
        }
    }

    public static bool TryWriteV6Block(this ref SpanWriter<char> writer, scoped ReadOnlySpan<ushort> values)
    {
        if (values.IsEmpty)
            return true;
        if (!writer.TryWriteHex16(values[0]))
            return false;
        for (int i = 1; i < values.Length; ++i)
        {
            if (!writer.TryWrite(':') || !writer.TryWriteHex16(values[i]))
                return false;
        }
        return true;
    }

    public static bool TryWriteV6Block(this ref SpanWriter<byte> writer, scoped ReadOnlySpan<ushort> values)
    {
        if (values.IsEmpty)
            return true;
        if (!writer.TryWriteHex16(values[0]))
            return false;
        for (int i = 1; i < values.Length; ++i)
        {
            if (!writer.TryWrite((byte)':') || !writer.TryWriteHex16(values[i]))
                return false;
        }
        return true;
    }
}