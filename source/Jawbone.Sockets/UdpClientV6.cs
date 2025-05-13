using System;

namespace Jawbone.Sockets;

public static class UdpClientV6
{
    public static IUdpClient<IpAddressV6> Connect(IpEndpoint<IpAddressV6> endpoint)
    {
        if (OperatingSystem.IsWindows())
            return Windows.WindowsUdpClientV6.Connect(endpoint);
        if (OperatingSystem.IsMacOS())
            return Mac.MacUdpClientV6.Connect(endpoint);
        else if (OperatingSystem.IsLinux())
            return Linux.LinuxUdpClientV6.Connect(endpoint);
        throw new PlatformNotSupportedException();
    }
}
