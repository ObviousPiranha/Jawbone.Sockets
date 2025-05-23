using System;
using System.Diagnostics.CodeAnalysis;

namespace Jawbone.Sockets;

static class ThrowExceptionFor
{
    [DoesNotReturn]
    public static void WrongAddressFamily()
    {
        throw new InvalidOperationException("Incorrect address family.");
    }

    [DoesNotReturn]
    public static void PollSocketError()
    {
        throw new InvalidOperationException("Socket reported error state.");
    }

    [DoesNotReturn]
    public static void BadPollState()
    {
        throw new InvalidOperationException("Unexpected poll event state.");
    }

    [DoesNotReturn]
    public static void InvalidNetwork<TAddress>(TAddress address, int prefixLength)
        where TAddress : unmanaged, IIpAddress<TAddress>
    {
        throw new ArgumentException($"Invalid mask address: {address}/{prefixLength}");
    }
}
