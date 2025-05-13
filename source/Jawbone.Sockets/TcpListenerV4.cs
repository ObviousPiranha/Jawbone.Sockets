using System;

namespace Jawbone.Sockets;

public static class TcpListenerV4
{
    public static ITcpListener<IpAddressV4> ListenLocalIp(int backlog) => Listen(IpAddressV4.Local.OnAnyPort(), backlog);
    public static ITcpListener<IpAddressV4> ListenAnyIp(int backlog) => Listen(default, backlog);
    public static ITcpListener<IpAddressV4> Listen(IpEndpoint<IpAddressV4> bindEndpoint, int backlog)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(backlog);
        if (OperatingSystem.IsWindows())
            return Windows.WindowsTcpListenerV4.Listen(bindEndpoint, backlog);
        if (OperatingSystem.IsMacOS())
            return Mac.MacTcpListenerV4.Listen(bindEndpoint, backlog);
        if (OperatingSystem.IsLinux())
            return Linux.LinuxTcpListenerV4.Listen(bindEndpoint, backlog);
        throw new PlatformNotSupportedException();
    }
}
