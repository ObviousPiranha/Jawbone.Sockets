using System;

namespace Jawbone.Sockets;

public interface IIpAddress
{
    bool IsDefault { get; }
    bool IsLinkLocal { get; }
    bool IsLoopback { get; }
}

public interface IIpAddress<TAddress> : IEquatable<TAddress>, ISpanParsable<TAddress>, ISpanFormattable, IUtf8SpanFormattable, IIpAddress
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    bool TryFormat(Span<char> destination, out int charsWritten);
    bool TryFormat(Span<byte> utf8Destination, out int bytesWritten);
    static abstract TAddress Parse(ReadOnlySpan<char> s);
    static abstract bool TryParse(ReadOnlySpan<char> s, out TAddress result);
    static abstract TAddress Any { get; }
    static abstract TAddress Local { get; }
}
