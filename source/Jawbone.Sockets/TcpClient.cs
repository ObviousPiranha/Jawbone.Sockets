namespace Jawbone.Sockets;

public static class TcpClient
{
    public static ITcpClient<TAddress> Connect<TAddress>(
        IpEndpoint<TAddress> ipEndpoint,
        SocketOptions socketOptions = default)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.ConnectTcpClient(ipEndpoint, socketOptions);
    }
}
