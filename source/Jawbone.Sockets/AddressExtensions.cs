namespace Jawbone.Sockets;

public static class AddressExtensions
{
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
}
