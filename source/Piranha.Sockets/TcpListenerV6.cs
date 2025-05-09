using System;

namespace Piranha.Sockets;

public static class TcpListenerV6
{
    public static ITcpListener<AddressV6> ListenAnyIp(int backlog, bool allowV4 = false) => Listen(default, backlog, allowV4);
    public static ITcpListener<AddressV6> ListenLocalIp(int backlog, bool allowV4 = false) => Listen(AddressV6.Local.OnAnyPort(), backlog, allowV4);
    public static ITcpListener<AddressV6> Listen(Endpoint<AddressV6> bindEndpoint, int backlog, bool allowV4 = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(backlog);
        if (OperatingSystem.IsWindows())
            return Windows.WindowsTcpListenerV6.Listen(bindEndpoint, backlog, allowV4);
        if (OperatingSystem.IsMacOS())
            return Mac.MacTcpListenerV6.Listen(bindEndpoint, backlog, allowV4);
        if (OperatingSystem.IsLinux())
            return Linux.LinuxTcpListenerV6.Listen(bindEndpoint, backlog, allowV4);
        throw new PlatformNotSupportedException();
    }
}
