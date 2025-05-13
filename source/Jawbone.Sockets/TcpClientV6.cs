using System;

namespace Jawbone.Sockets;

public static class TcpClientV6
{
    public static ITcpClient<IpAddressV6> Connect(IpEndpoint<IpAddressV6> endpoint)
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
