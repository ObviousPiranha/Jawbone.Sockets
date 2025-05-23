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

    public bool TryWrite(T item)
    {
        if (Span.Length <= _position)
            return false;
        Span[_position++] = item;
        return true;
    }

    public bool TryWrite(params ReadOnlySpan<T> items)
    {
        if (!items.TryCopyTo(Free))
            return false;
        _position += items.Length;
        return true;
    }

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

    public static bool TryWriteFormattable<T>(
        ref this SpanWriter<byte> writer,
        T item,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default) where T : IUtf8SpanFormattable
    {
        var result = item.TryFormat(writer.Free, out var bytesWritten, format, provider);
        writer.Position += bytesWritten;
        return result;
    }

    public static bool TryWriteFormattable<T>(
        ref this SpanWriter<char> writer,
        T item,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = default) where T : ISpanFormattable
    {
        var result = item.TryFormat(writer.Free, out var charsWritten, format, provider);
        writer.Position += charsWritten;
        return result;
    }

    public static string GetString<T>(T item, int bufferSize = 64) where T : ISpanFormattable
    {
        Span<char> buffer = stackalloc char[bufferSize];
        _ = item.TryFormat(buffer, out var n, default, default);
        return buffer[..n].ToString();
    }
}