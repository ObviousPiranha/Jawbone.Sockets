using System;
using System.Text;

namespace Jawbone.Sockets.Samples;

static class SpanWriter
{
    public static SpanWriter<T> Create<T>(Span<T> span) => new(span);
    public static SpanWriter<T> Create<T>(T[] span) => new(span);

    public static void WriteAsUtf8(ref this SpanWriter<byte> writer, ReadOnlySpan<char> utf16)
    {
        Encoding.UTF8.TryGetBytes(utf16, writer.Free, out var bytesWritten);
        writer.Advance(bytesWritten);
    }
}

ref struct SpanWriter<T>
{
    private int _position;
    public readonly Span<T> Span { get; }
    public int Position
    {
        get => _position;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, Span.Length);
            _position = value;
        }
    }

    public readonly Span<T> Written => Span[.._position];
    public readonly Span<T> Free => Span[_position..];

    public SpanWriter(Span<T> span) => Span = span;

    public void Reset() => _position = 0;
    public void Advance(int count) => Position = _position + count;

    public void Write(ReadOnlySpan<T> span)
    {
        span.CopyTo(Free);
        _position += span.Length;
    }

    public void Write(T value)
    {
        Span[_position] = value;
        ++_position;
    }
}