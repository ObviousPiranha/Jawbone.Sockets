using System;
using System.Collections.Immutable;

namespace Jawbone.Sockets;

public readonly struct AddressInfo
{
    public DateTimeOffset CreatedAt { get; init; }
    public readonly string? Node { get; init; }
    public readonly string? Service { get; init; }
    public readonly ImmutableArray<IpEndpoint<IpAddressV4>> V4 { get; init; }
    public readonly ImmutableArray<IpEndpoint<IpAddressV6>> V6 { get; init; }

    public readonly bool IsEmpty => V4.IsDefaultOrEmpty && V6.IsDefaultOrEmpty;

    public static AddressInfo Get(
        string? node,
        string? service = null,
        TimeProvider? timeProvider = null)
    {
        if (OperatingSystem.IsWindows())
            return Windows.WindowsAddressInfo.Get(node, service, timeProvider);
        if (OperatingSystem.IsMacOS())
            return Mac.MacAddressInfo.Get(node, service, timeProvider);
        if (OperatingSystem.IsLinux())
            return Linux.LinuxAddressInfo.Get(node, service, timeProvider);
        throw new PlatformNotSupportedException();
    }
}
