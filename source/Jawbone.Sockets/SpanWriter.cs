using System;

namespace Jawbone.Sockets;

ref struct SpanWriter<T>
{
    public readonly Span<T> Span { get; }
    private int _position;

    public int Position
    {
        readonly get => _position;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Span.Length);
            _position = value;
        }
    }

    public readonly ReadOnlySpan<T> Written => Span[.._position];
    public readonly Span<T> Free => Span[_position..];

    public SpanWriter(Span<T> span) => Span = span;

    public void Write(T item)
    {
        Span[_position] = item;
        ++_position;
    }

    public void Write(params ReadOnlySpan<T> items)
    {
        items.CopyTo(Free);
        _position += items.Length;
    }
} 

static class SpanWriter
{
    public static SpanWriter<T> Create<T>(Span<T> span) => new(span);
    public static SpanWriter<T> Create<T>(T[]? array) => new(array);

    public static void WriteBase10(ref this SpanWriter<char> writer, uint value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException("Not enough room to write value into span.");
        writer.Position += charsWritten;
    }

    public static void WriteBase10(ref this SpanWriter<byte> writer, uint value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException("Not enough room to write value into span.");
        writer.Position += charsWritten;
    }

    public static void WriteIpAddress(
        ref this SpanWriter<char> writer,
        IpAddress address)
    {
        var n = address.FormatUtf16(writer.Free);
        writer.Position += n;
    }

    public static void WriteIpAddress(
        ref this SpanWriter<byte> writer,
        IpAddress address)
    {
        var n = address.FormatUtf8(writer.Free);
        writer.Position += n;
    }

    public static void WriteIpAddress<TAddress>(
        ref this SpanWriter<char> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        var n = address.FormatUtf16(writer.Free);
        writer.Position += n;
    }

    public static void WriteIpAddress<TAddress>(
        ref this SpanWriter<byte> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        var n = address.FormatUtf8(writer.Free);
        writer.Position += n;
    }
}