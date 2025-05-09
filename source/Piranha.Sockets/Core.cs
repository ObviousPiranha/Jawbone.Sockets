using System;
using System.Diagnostics.CodeAnalysis;

namespace Piranha.Sockets;

static class Core
{
    public static int GetMilliseconds(TimeSpan timeSpan)
    {
        var ms64 = timeSpan.Ticks / TimeSpan.TicksPerMillisecond;
        var clamped = long.Clamp(ms64, 0, int.MaxValue);
        return (int)clamped;
    }
}
