using System;
using System.Text;

namespace Jawbone.Sockets;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIpAddress(
        this StringBuilder builder,
        in IpAddress address)
    {
        return AppendFormattable(builder, address);
    }

    public static StringBuilder AppendIpAddress<TAddress>(
        this StringBuilder builder,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return AppendFormattable(builder, address);
    }

    public static StringBuilder AppendIpEndpoint<TAddress>(
        this StringBuilder builder,
        IpEndpoint endpoint)
    {
        return AppendFormattable(builder, endpoint);
    }

    public static StringBuilder AppendIpEndpoint<TAddress>(
        this StringBuilder builder,
        IpEndpoint<TAddress> endpoint) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return AppendFormattable(builder, endpoint);
    }

    private static StringBuilder AppendFormattable<T>(StringBuilder builder, T item) where T : ISpanFormattable
    {
        Span<char> buffer = stackalloc char[64];
        _ = item.TryFormat(buffer, out var n, default, default);
        return builder.Append(buffer[..n]);
    }
}
