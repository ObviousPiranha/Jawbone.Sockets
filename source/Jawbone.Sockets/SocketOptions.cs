using System;

namespace Jawbone.Sockets;

[Flags]
public enum SocketOptions
{
    None,
    EnableUdpBroadcast = 1 << 0,
    DoNotReuseAddress = 1 << 1,
    DisableTcpNoDelay = 1 << 2,
    EnableDualMode = 1 << 3
}

public static class SocketOptionsExtensions
{
    public static bool All(
        this SocketOptions source,
        SocketOptions mask)
    {
        return (source & mask) == mask;
    }

    public static bool Any(
        this SocketOptions source,
        SocketOptions mask)
    {
        return (source & mask) != SocketOptions.None;
    }

    public static bool None(
        this SocketOptions source,
        SocketOptions mask)
    {
        return (source & mask) == SocketOptions.None;
    }
}
