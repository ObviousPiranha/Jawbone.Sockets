using System;

namespace Jawbone.Sockets;

ref struct SpanReader<T>
{
    public readonly ReadOnlySpan<T> Span { get; }
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

    public readonly ReadOnlySpan<T> Consumed => Span[.._position];
    public readonly ReadOnlySpan<T> Remaining => Span[_position..];
    public readonly bool AtEnd => _position == Span.Length;

    public SpanReader(ReadOnlySpan<T> span) => Span = span;
}

static class SpanReader
{
    public static SpanReader<T> Create<T>(Span<T> span) => new(span);
    public static SpanReader<T> Create<T>(ReadOnlySpan<T> span) => new(span);
    public static SpanReader<T> Create<T>(T[]? array) => new(array);
    public static SpanReader<T> Create<T>(ArraySegment<T> segment) => new(segment);
    public static SpanReader<char> Create(string? s) => new(s);

    public static bool TryMatch<T>(
        ref this SpanReader<T> reader,
        T item) where T : IEquatable<T>
    {
        if (reader.Position < reader.Span.Length && reader.Span[reader.Position].Equals(item))
        {
            ++reader.Position;
            return true;
        }

        return false;
    }

    public static bool TryMatch<T>(
        ref this SpanReader<T> reader,
        ReadOnlySpan<T> items) where T : IEquatable<T>
    {
        if (reader.Position < reader.Span.Length && reader.Span.StartsWith(items))
        {
            reader.Position += items.Length;
            return true;
        }

        return false;
    }

    public static bool TryParseByte(
        ref this SpanReader<char> reader,
        out byte value)
    {
        var span = reader.Remaining;
        if (span.IsEmpty || !IsDigit(span[0]))
        {
            value = default;
            return false;
        }

        int result = span[0] - '0';
        int i = 1;

        for (; i < span.Length; ++i)
        {
            int c = span[i];

            if (!IsDigit(c))
            {
                reader.Position += i;
                value = (byte)result;
                return true;
            }

            result = result * 10 + (c - '0');
            if (byte.MaxValue < result)
            {
                value = default;
                return false;
            }
        }

        reader.Position += i;
        value = (byte)result;
        return true;
    }

    public static bool TryParseByte(
        ref this SpanReader<byte> reader,
        out byte value)
    {
        var span = reader.Remaining;
        if (span.IsEmpty || !IsDigit(span[0]))
        {
            value = default;
            return false;
        }

        int result = span[0] - '0';
        int i = 1;

        for (; i < span.Length; ++i)
        {
            int c = span[i];

            if (!IsDigit(c))
            {
                reader.Position += i;
                value = (byte)result;
                return true;
            }

            result = result * 10 + (c - '0');
            if (byte.MaxValue < result)
            {
                value = default;
                return false;
            }
        }

        reader.Position += i;
        value = (byte)result;
        return true;
    }

    private static bool IsDigit(int c) => '0' <= c && c <= '9';
}