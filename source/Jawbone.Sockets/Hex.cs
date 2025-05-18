using System;

namespace Jawbone.Sockets;

static class Hex
{
    public const string Digits = "0123456789abcdef";
    public static ReadOnlySpan<byte> Digits8 => "0123456789abcdef"u8;

    public static void WriteFullHex8(ref this SpanWriter<char> writer, int value)
    {
        writer.Write(Digits[(value >> 4) & 0xf]);
        writer.Write(Digits[value & 0xf]);
    }

    public static void WriteFullHex8(ref this SpanWriter<byte> writer, int value)
    {
        writer.Write(Digits8[(value >> 4) & 0xf]);
        writer.Write(Digits8[value & 0xf]);
    }

    public static void WriteHex8(ref this SpanWriter<char> writer, int value)
    {
        var hi = (value >> 4) & 0xf;
        if (0 < hi)
            writer.Write(Digits[hi]);
        writer.Write(Digits[value & 0xf]);
    }

    public static void WriteHex8(ref this SpanWriter<byte> writer, int value)
    {
        var hi = (value >> 4) & 0xf;
        if (0 < hi)
            writer.Write(Digits8[hi]);
        writer.Write(Digits8[value & 0xf]);
    }

    public static void WriteHex16(ref this SpanWriter<char> writer, int hi, int lo)
    {
        if (0 < hi)
        {
            writer.WriteHex8(hi);
            writer.WriteFullHex8(lo);
        }
        else
        {
            writer.WriteHex8(lo);
        }
    }

    public static void WriteHex16(ref this SpanWriter<byte> writer, int hi, int lo)
    {
        if (0 < hi)
        {
            writer.WriteHex8(hi);
            writer.WriteFullHex8(lo);
        }
        else
        {
            writer.WriteHex8(lo);
        }
    }

    public static void WriteHex16(ref this SpanWriter<char> writer, int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var hi = value & 0xff;
            var lo = (value >> 8) & 0xff;
            writer.WriteHex16(hi, lo);
        }
        else
        {
            var hi = (value >> 8) & 0xff;
            var lo = value & 0xff;
            writer.WriteHex16(hi, lo);
        }
    }

    public static void WriteHex16(ref this SpanWriter<byte> writer, int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            var hi = value & 0xff;
            var lo = (value >> 8) & 0xff;
            writer.WriteHex16(hi, lo);
        }
        else
        {
            var hi = (value >> 8) & 0xff;
            var lo = value & 0xff;
            writer.WriteHex16(hi, lo);
        }
    }

    public static void WriteV6Block(this ref SpanWriter<char> writer, scoped ReadOnlySpan<ushort> values)
    {
        if (values.IsEmpty)
            return;
        writer.WriteHex16(values[0]);
        for (int i = 1; i < values.Length; ++i)
        {
            writer.Write(':');
            writer.WriteHex16(values[i]);
        }
    }

    public static void WriteV6Block(this ref SpanWriter<byte> writer, scoped ReadOnlySpan<ushort> values)
    {
        if (values.IsEmpty)
            return;
        writer.WriteHex16(values[0]);
        for (int i = 1; i < values.Length; ++i)
        {
            writer.Write((byte)':');
            writer.WriteHex16(values[i]);
        }
    }
}