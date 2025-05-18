using System;
using System.Text;

namespace Jawbone.Sockets;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIpAddress(
        this StringBuilder builder,
        IpAddress address)
    {
        Span<char> buffer = stackalloc char[64];
        var writer = SpanWriter.Create(buffer);
        writer.WriteIpAddress(address);
        return builder.Append(writer.Written);
    }

    public static StringBuilder AppendIpAddress<TAddress>(
        this StringBuilder builder,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        Span<char> buffer = stackalloc char[64];
        var n = address.FormatUtf16(buffer);
        return builder.Append(buffer[..n]);
    }

    public static StringBuilder AppendIpEndpoint<TAddress>(
        this StringBuilder builder,
        IpEndpoint<TAddress> endpoint) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        Span<char> buffer = stackalloc char[64];
        var n = endpoint.FormatUtf16(buffer);
        return builder.Append(buffer[..n]);
    }
}
