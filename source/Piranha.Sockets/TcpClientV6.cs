using System;

namespace Piranha.Sockets;

public static class TcpClientV6
{
    public static ITcpClient<AddressV6> Connect(Endpoint<AddressV6> endpoint)
    {
        if (OperatingSystem.IsWindows())
            return Windows.WindowsTcpClientV6.Connect(endpoint);
        if (OperatingSystem.IsMacOS())
            return Mac.MacTcpClientV6.Connect(endpoint);
        if (OperatingSystem.IsLinux())
            return Linux.LinuxTcpClientV6.Connect(endpoint);
        throw new PlatformNotSupportedException();
    }
}
