using System;

namespace Jawbone.Sockets;

public static class UdpClientV4
{
    public static IUdpClient<AddressV4> Connect(Endpoint<AddressV4> endpoint)
    {
        if (OperatingSystem.IsWindows())
            return Windows.WindowsUdpClientV4.Connect(endpoint);
        if (OperatingSystem.IsMacOS())
            return Mac.MacUdpClientV4.Connect(endpoint);
        if (OperatingSystem.IsLinux())
            return Linux.LinuxUdpClientV4.Connect(endpoint);
        throw new PlatformNotSupportedException();
    }
}
