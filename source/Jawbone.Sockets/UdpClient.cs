namespace Jawbone.Sockets;

public static class UdpClient
{
    public static IUdpClient<TAddress> Connect<TAddress>(
        IpEndpoint<TAddress> ipEndpoint,
        SocketOptions socketOptions = default)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.ConnectUdpClient(ipEndpoint, socketOptions);
    }
}
