using System;
using System.Collections.Generic;
using System.Linq;

namespace Jawbone.Sockets;

public static class Dns
{
    private static IEnumerable<IpEndpoint> CreateQuery(
        string? node,
        string? service,
        IpAddressVersion filter)
    {
        if (OperatingSystem.IsWindows())
            return new Windows.WindowsDns.Enumerable(node, service, filter);
        if (OperatingSystem.IsMacOS())
            return new Mac.MacDns.Enumerable(node, service, filter);
        if (OperatingSystem.IsLinux())
            return new Linux.LinuxDns.Enumerable(node, service, filter);
        throw new PlatformNotSupportedException();
    }

    public static IEnumerable<IpEndpoint> Query(string? node, string? service = null)
    {
        return CreateQuery(node, service, IpAddressVersion.None);
    }

    public static IEnumerable<IpEndpoint<IpAddressV4>> QueryV4(string? node, string? service = null)
    {
        return CreateQuery(node, service, IpAddressVersion.V4)
            .Select(static endpoint => endpoint.AsV4());
    }

    public static IEnumerable<IpEndpoint<IpAddressV6>> QueryV6(string? node, string? service = null)
    {
        return CreateQuery(node, service, IpAddressVersion.V6)
            .Select(static endpoint => endpoint.AsV6());
    }

    public static IpEndpoint<IpAddressV4> GetEndpointV4(string? node) => QueryV4(node).First();
    public static IpEndpoint<IpAddressV6> GetEndpointV6(string? node) => QueryV6(node).First();
    public static IpAddressV4 GetAddressV4(string? node) => QueryV4(node).First().Address;
    public static IpAddressV6 GetAddressV6(string? node) => QueryV6(node).First().Address;

    public static bool TryGetAddressV4(string? node, out IpAddressV4 address)
    {
        foreach (var endpoint in QueryV4(node))
        {
            address = endpoint.Address;
            return true;
        }

        address = default;
        return false;
    }

    public static bool TryGetAddressV6(string? node, out IpAddressV6 address)
    {
        foreach (var endpoint in QueryV6(node))
        {
            address = endpoint.Address;
            return true;
        }

        address = default;
        return false;
    }
}
