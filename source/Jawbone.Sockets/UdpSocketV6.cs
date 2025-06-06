using System;

namespace Jawbone.Sockets;

public static class UdpSocketV6
{

    public static IUdpSocket<IpAddressV6> BindAnyIp(int port, bool allowV4 = false) => BindAnyIp((NetworkPort)port, allowV4);
    public static IUdpSocket<IpAddressV6> BindAnyIp(NetworkPort port, bool allowV4 = false) => Bind(new(default, port), allowV4);
    public static IUdpSocket<IpAddressV6> BindAnyIp(bool allowV4 = false) => Bind(default, allowV4);
    public static IUdpSocket<IpAddressV6> BindLocalIp(int port, bool allowV4 = false) => Bind(new(IpAddressV6.Local, (NetworkPort)port), allowV4);
    public static IUdpSocket<IpAddressV6> BindLocalIp(NetworkPort port, bool allowV4 = false) => Bind(new(IpAddressV6.Local, port), allowV4);
    public static IUdpSocket<IpAddressV6> BindLocalIp(bool allowV4 = false) => Bind(new(IpAddressV6.Local, default(NetworkPort)), allowV4);
    public static IUdpSocket<IpAddressV6> Bind(IpEndpoint<IpAddressV6> endpoint, bool allowV4 = false)
    {
        if (OperatingSystem.IsWindows())
        {
            return Windows.WindowsUdpSocketV6.Bind(endpoint, allowV4);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Mac.MacUdpSocketV6.Bind(endpoint, allowV4);
        }
        else if (OperatingSystem.IsLinux())
        {
            return Linux.LinuxUdpSocketV6.Bind(endpoint, allowV4);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    // TODO: Remove.
    private static IUdpSocket<IpAddressV6> Create(bool allowV4 = false)
    {
        // https://stackoverflow.com/a/17922652
        if (OperatingSystem.IsWindows())
        {
            return Windows.WindowsUdpSocketV6.Create(allowV4);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Mac.MacUdpSocketV6.Create(allowV4);
        }
        else if (OperatingSystem.IsLinux())
        {
            return Linux.LinuxUdpSocketV6.Create(allowV4);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}
