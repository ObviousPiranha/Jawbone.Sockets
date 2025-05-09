using System;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Sockets;

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
}
