using System;

namespace Piranha.Sockets;

public static class UdpSocketV4
{
    public static IUdpSocket<AddressV4> BindAnyIp(int port) => BindAnyIp((NetworkPort)port);
    public static IUdpSocket<AddressV4> BindAnyIp(NetworkPort port) => Bind(new(default, port));
    public static IUdpSocket<AddressV4> BindAnyIp() => Bind(default);
    public static IUdpSocket<AddressV4> BindLocalIp(int port) => Bind(new(AddressV4.Local, (NetworkPort)port));
    public static IUdpSocket<AddressV4> BindLocalIp(NetworkPort port) => Bind(new(AddressV4.Local, port));
    public static IUdpSocket<AddressV4> BindLocalIp() => Bind(new(AddressV4.Local, default(NetworkPort)));
    public static IUdpSocket<AddressV4> Bind(Endpoint<AddressV4> endpoint)
    {
        if (OperatingSystem.IsWindows())
        {
            return Windows.WindowsUdpSocketV4.Bind(endpoint);
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Mac.MacUdpSocketV4.Bind(endpoint);
        }
        else if (OperatingSystem.IsLinux())
        {
            return Linux.LinuxUdpSocketV4.Bind(endpoint);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    public static IUdpSocket<AddressV4> Create()
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
