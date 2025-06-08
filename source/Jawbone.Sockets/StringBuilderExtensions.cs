using System;
using System.Diagnostics;
using System.Text;

namespace Jawbone.Sockets;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIpAddress(
        this StringBuilder builder,
        in IpAddress ipAddress)
    {
        return AppendFormattable(builder, ipAddress);
    }

    public static StringBuilder AppendIpAddress<TAddress>(
        this StringBuilder builder,
        TAddress ipAddress) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return AppendFormattable(builder, ipAddress);
    }

    public static StringBuilder AppendIpEndpoint(
        this StringBuilder builder,
        IpEndpoint ipEndpoint)
    {
        return AppendFormattable(builder, ipEndpoint);
    }

    public static StringBuilder AppendIpEndpoint<TAddress>(
        this StringBuilder builder,
        IpEndpoint<TAddress> ipEndpoint) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return AppendFormattable(builder, ipEndpoint);
    }

    public static StringBuilder AppendIpNetwork(
        this StringBuilder builder,
        IpNetwork ipNetwork)
    {
        return AppendFormattable(builder, ipNetwork);
    }

    public static StringBuilder AppendIpNetwork<TAddress>(
        this StringBuilder builder,
        IpNetwork<TAddress> ipNetwork) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return AppendFormattable(builder, ipNetwork);
    }

    private static StringBuilder AppendFormattable<T>(StringBuilder builder, T item) where T : ISpanFormattable
    {
        Span<char> buffer = stackalloc char[64];
        var formatted = item.TryFormat(buffer, out var n, default, default);
        Debug.Assert(formatted);
        return builder.Append(buffer[..n]);
    }
}
