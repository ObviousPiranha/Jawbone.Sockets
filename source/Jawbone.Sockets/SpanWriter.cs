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

    public static void WriteBase10(ref this SpanWriter<char> writer, uint value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += charsWritten;
    }

    public static void WriteBase10(ref this SpanWriter<byte> writer, uint value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += charsWritten;
    }

    public static void WriteBase10(ref this SpanWriter<char> writer, byte value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += charsWritten;
    }

    public static void WriteBase10(ref this SpanWriter<byte> writer, byte value)
    {
        if (!value.TryFormat(writer.Free, out var charsWritten))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += charsWritten;
    }

    public static bool TryWriteBase10(ref this SpanWriter<char> writer, byte value)
    {
        var result = value.TryFormat(writer.Free, out var charsWritten);
        writer.Position += charsWritten;
        return result;
    }

    public static bool TryWriteBase10(ref this SpanWriter<byte> writer, byte value)
    {
        var result = value.TryFormat(writer.Free, out var charsWritten);
        writer.Position += charsWritten;
        return result;
    }

    public static bool TryWriteBase10(ref this SpanWriter<char> writer, uint value)
    {
        var result = value.TryFormat(writer.Free, out var charsWritten);
        writer.Position += charsWritten;
        return result;
    }

    public static bool TryWriteBase10(ref this SpanWriter<byte> writer, uint value)
    {
        var result = value.TryFormat(writer.Free, out var charsWritten);
        writer.Position += charsWritten;
        return result;
    }

    public static void WriteIpAddress(
        ref this SpanWriter<char> writer,
        IpAddress address)
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += n;
    }

    public static void WriteIpAddress(
        ref this SpanWriter<byte> writer,
        in IpAddress address)
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += n;
    }

    public static void WriteIpAddress<TAddress>(
        ref this SpanWriter<char> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += n;
    }

    public static void WriteIpAddress<TAddress>(
        ref this SpanWriter<byte> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            throw new InvalidOperationException(ExceptionMessages.SpanRoom);
        writer.Position += n;
    }

    public static bool TryWriteIpAddress(
        ref this SpanWriter<char> writer,
        in IpAddress address)
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            return false;
        writer.Position += n;
        return true;
    }

    public static bool TryWriteIpAddress(
        ref this SpanWriter<byte> writer,
        in IpAddress address)
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            return false;
        writer.Position += n;
        return true;
    }

    public static bool TryWriteIpAddress<TAddress>(
        ref this SpanWriter<char> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            return false;
        writer.Position += n;
        return true;
    }

    public static bool TryWriteIpAddress<TAddress>(
        ref this SpanWriter<byte> writer,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        if (!address.TryFormat(writer.Free, out var n, default, default))
            return false;
        writer.Position += n;
        return true;
    }
}