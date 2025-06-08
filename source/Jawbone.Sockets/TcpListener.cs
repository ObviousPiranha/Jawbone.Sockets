namespace Jawbone.Sockets;

public static class TcpListener
{
    public static ITcpListener<IpAddressV4> ListenLocalIpV4(
        int backlog,
        SocketOptions socketOptions = default)
    {
        return Listen(IpAddressV4.Local.OnAnyPort(), backlog, socketOptions);
    }

    public static ITcpListener<IpAddressV4> ListenAnyIpV4(
        int backlog,
        SocketOptions socketOptions = default)
    {
        return Listen(IpAddressV4.Any.OnAnyPort(), backlog, socketOptions);
    }

    public static ITcpListener<IpAddressV6> ListenLocalIpV6(
        int backlog,
        SocketOptions socketOptions = default)
    {
        return Listen(IpAddressV6.Local.OnAnyPort(), backlog, socketOptions);
    }

    public static ITcpListener<IpAddressV6> ListenAnyIpV6(
        int backlog,
        SocketOptions socketOptions = default)
    {
        return Listen(IpAddressV6.Any.OnAnyPort(), backlog, socketOptions);
    }

    public static ITcpListener<TAddress> Listen<TAddress>(
        IpEndpoint<TAddress> bindEndpoint,
        int backlog,
        SocketOptions socketOptions = default)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.TcpListen(bindEndpoint, backlog, socketOptions);
    }
}
