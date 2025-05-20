using System;
using System.Text;

namespace Jawbone.Sockets;

public interface IIpAddress
{
    bool IsDefault { get; }
    bool IsLinkLocal { get; }
    bool IsLoopback { get; }
    int FormatUtf16(Span<char> utf16);
    int FormatUtf8(Span<byte> utf8);
}

public interface IIpAddress<TAddress> : IEquatable<TAddress>, ISpanParsable<TAddress>, IIpAddress
    where TAddress : unmanaged, IIpAddress<TAddress>
{
    static abstract TAddress Parse(ReadOnlySpan<char> s);
    static abstract bool TryParse(ReadOnlySpan<char> s, out TAddress result);
    static abstract TAddress Any { get; }
    static abstract TAddress Local { get; }
}
