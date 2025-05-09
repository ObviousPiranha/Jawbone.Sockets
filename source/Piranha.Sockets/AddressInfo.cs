using Piranha.Sockets.Linux;
using Piranha.Sockets.Mac;
using Piranha.Sockets.Windows;
using System;
using System.Collections.Immutable;

namespace Piranha.Sockets;

public readonly struct AddressInfo
{
    public DateTimeOffset CreatedAt { get; init; }
    public readonly string? Node { get; init; }
    public readonly string? Service { get; init; }
    public readonly ImmutableArray<Endpoint<AddressV4>> V4 { get; init; }
    public readonly ImmutableArray<Endpoint<AddressV6>> V6 { get; init; }

    public readonly bool IsEmpty => V4.IsDefaultOrEmpty && V6.IsDefaultOrEmpty;

    public static AddressInfo Get(
        string? node,
        string? service = null,
        TimeProvider? timeProvider = null)
    {
        if (OperatingSystem.IsWindows())
            return WindowsAddressInfo.Get(node, service, timeProvider);
        if (OperatingSystem.IsMacOS())
            return MacAddressInfo.Get(node, service, timeProvider);
        if (OperatingSystem.IsLinux())
            return LinuxAddressInfo.Get(node, service, timeProvider);
        throw new PlatformNotSupportedException();
    }
}
