using System;

namespace Jawbone.Sockets;

public static class UdpSocket
{
    public static IUdpSocket<IpAddressV4> BindAnyIpV4(
        int port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Any.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV4> BindAnyIpV4(
        NetworkPort port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Any.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV4> BindAnyIpV4(
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Any.OnAnyPort(), socketOptions);
    }

    public static IUdpSocket<IpAddressV4> BindLocalIpV4(
        int port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Local.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV4> BindLocalIpV4(
        NetworkPort port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Local.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV4> BindLocalIpV4(
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV4.Local.OnAnyPort(), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindAnyIpV6(
        int port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Any.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindAnyIpV6(
        NetworkPort port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Any.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindAnyIpV6(
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Any.OnAnyPort(), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindLocalIpV6(
        int port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Local.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindLocalIpV6(
        NetworkPort port,
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Local.OnPort(port), socketOptions);
    }

    public static IUdpSocket<IpAddressV6> BindLocalIpV6(
        SocketOptions socketOptions = default)
    {
        return Bind(IpAddressV6.Local.OnAnyPort(), socketOptions);
    }

    public static IUdpSocket<TAddress> Bind<TAddress>(
        IpEndpoint<TAddress> ipEndpoint,
        SocketOptions socketOptions = default)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        return TAddress.BindUdpSocket(ipEndpoint, socketOptions);
    }

    // TODO: Remove.
    private static IUdpSocket<IpAddressV4> Create()
    {
        // https://stackoverflow.com/a/17922652
        if (OperatingSystem.IsWindows())
        {
            return Windows.WindowsUdpSocketV4.Create();
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Mac.MacUdpSocketV4.Create();
        }
        else if (OperatingSystem.IsLinux())
        {
            return Linux.LinuxUdpSocketV4.Create();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}
