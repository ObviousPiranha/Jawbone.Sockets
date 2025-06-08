namespace Jawbone.Sockets;

public static class IpAddressExtensions
{
    public static IpAddressV6 WithScopeId(this IpAddressV6 ipAddress, uint scopeId)
    {
        ipAddress.ScopeId = scopeId;
        return ipAddress;
    }

    public static IpEndpoint<TAddress> OnAnyPort<TAddress>(
        this TAddress address
        ) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return new(address, default(NetworkPort));
    }

    public static IpEndpoint<TAddress> OnPort<TAddress>(
        this TAddress address,
        int port
        ) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return new(address, port);
    }

    public static IpEndpoint<TAddress> OnPort<TAddress>(
        this TAddress address,
        NetworkPort port
        ) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return new(address, port);
    }

    public static IpEndpoint OnAnyPort(this IpAddress address) => new(address, default(NetworkPort));
    public static IpEndpoint OnPort(this IpAddress address, int port) => new(address, port);
    public static IpEndpoint OnPort(this IpAddress address, NetworkPort port) => new(address, port);

    public static IpNetwork<TAddress> WithPrefix<TAddress>(
        this TAddress ipAddress,
        int prefixLength
        ) where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.CreateNetwork(ipAddress, prefixLength);
    }

    public static IpNetwork WithPrefix(this IpAddress ipAddress, int prefixLength) => IpAddress.CreateNetwork(ipAddress, prefixLength);
}
