namespace Jawbone.Sockets;

public static class AddressExtensions
{
    public static Endpoint<TAddress> OnAnyPort<TAddress>(
        this TAddress address
        ) where TAddress : unmanaged, IAddress<TAddress>
    {
        return new(address, default(NetworkPort));
    }

    public static Endpoint<TAddress> OnPort<TAddress>(
        this TAddress address,
        int port
        ) where TAddress : unmanaged, IAddress<TAddress>
    {
        return new(address, port);
    }

    public static Endpoint<TAddress> OnPort<TAddress>(
        this TAddress address,
        NetworkPort port
        ) where TAddress : unmanaged, IAddress<TAddress>
    {
        return new(address, port);
    }

    public static Endpoint OnAnyPort(this Address address) => new(address, default(NetworkPort));
    public static Endpoint OnPort(this Address address, int port) => new(address, port);
    public static Endpoint OnPort(this Address address, NetworkPort port) => new(address, port);
}
