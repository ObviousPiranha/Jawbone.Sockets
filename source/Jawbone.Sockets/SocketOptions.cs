using System;

namespace Jawbone.Sockets;

[Flags]
public enum SocketOptions
{
    None,
    ThrowExceptionForIrrelevantOptions = 1 << 0,
    DoNotReuseAddress = 1 << 1,
    DisableTcpNoDelay = 1 << 2,
    EnableDualMode = 1 << 3,
    EnableUdpBroadcast = 1 << 4
}

public static class SocketOptionsExtensions
{
    public static bool All(
        this SocketOptions source,
        SocketOptions options)
    {
        return (source & options) == options;
    }

    public static bool Any(
        this SocketOptions source,
        SocketOptions options)
    {
        return (source & options) != SocketOptions.None;
    }
}
