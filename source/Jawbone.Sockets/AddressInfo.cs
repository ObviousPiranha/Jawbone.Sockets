using System;
using System.Collections.Immutable;

namespace Jawbone.Sockets;

public readonly struct AddressInfo
{
    public DateTimeOffset CreatedAt { get; init; }
    public readonly string? Node { get; init; }
    public readonly string? Service { get; init; }
    public readonly ImmutableArray<IpEndpoint<IpAddressV4>> V4 { get; init; }
    public readonly ImmutableArray<IpEndpoint<IpAddressV6>> V6 { get; init; }

    public readonly bool IsEmpty => V4.IsDefaultOrEmpty && V6.IsDefaultOrEmpty;

    public static AddressInfo Get(
        string? node,
        string? service = null,
        TimeProvider? timeProvider = null)
    {
        var v4 = ImmutableArray.CreateBuilder<IpEndpoint<IpAddressV4>>();
        var v6 = ImmutableArray.CreateBuilder<IpEndpoint<IpAddressV6>>();

        foreach (var endpoint in Dns.Query(node, service))
        {
            if (endpoint.Address.Version == IpAddressVersion.V4)
                v4.Add(endpoint.AsV4());
            else if (endpoint.Address.Version == IpAddressVersion.V6)
                v6.Add(endpoint.AsV6());
        }

        timeProvider ??= TimeProvider.System;

        var result = new AddressInfo
        {
            CreatedAt = timeProvider.GetLocalNow(),
            Node = node,
            Service = service,
            V4 = v4.DrainToImmutable(),
            V6 = v6.DrainToImmutable()
        };

        return result;
    }
}
