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

    public ReadOnlySpan<T> Written => Span[.._position];
    public Span<T> Free => Span[_position..];

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

    public static void WriteHex(ref this SpanWriter<char> writer, byte value)
    {
        
    }
}