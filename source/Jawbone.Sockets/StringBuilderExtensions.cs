using System;
using System.Text;

namespace Jawbone.Sockets;

public static class StringBuilderExtensions
{
    private const string Hex = "0123456789abcdef";

    internal static StringBuilder AppendFullHex(
        this StringBuilder builder,
        byte value)
    {
        return builder
            .Append(Hex[(value >> 4) & 0xf])
            .Append(Hex[value & 0xf]);
    }

    internal static StringBuilder AppendCollapsedHex(
        this StringBuilder builder,
        byte value)
    {
        var index = (value >> 4) & 0xf;

        if (0 < index)
            builder.Append(Hex[index]);

        return builder.Append(Hex[value & 0xf]);
    }

    internal static StringBuilder AppendCollapsedHex(
        this StringBuilder builder,
        byte hi,
        byte lo)
    {
        if (0 < hi)
        {
            return builder
                .AppendCollapsedHex(hi)
                .AppendFullHex(lo);
        }
        else
        {
            return builder.AppendCollapsedHex(lo);
        }
    }

    internal static StringBuilder AppendV6Block(
        this StringBuilder builder,
        ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
            return builder;

        builder
            .AppendCollapsedHex(span[0], span[1]);

        for (int i = 2; i < span.Length; i += 2)
        {
            builder
                .Append(':')
                .AppendCollapsedHex(span[i], span[i + 1]);
        }

        return builder;
    }

    public static StringBuilder AppendAddress<TAddress>(
        this StringBuilder builder,
        TAddress address) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        address.AppendTo(builder);
        return builder;
    }

    public static StringBuilder AppendEndpoint<TAddress>(
        this StringBuilder builder,
        IpEndpoint<TAddress> endpoint) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return builder
            .Append('[')
            .AppendAddress(endpoint.Address)
            .Append("]:")
            .Append(endpoint.Port.HostValue);
    }
}
