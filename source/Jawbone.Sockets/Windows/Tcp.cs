using System;
using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Windows;

static class Tcp
{
    public const int NoDelay = 1;

    public static void SetNoDelay(nuint fd, bool enable)
    {
        var result = Sys.SetSockOpt(
            fd,
            IpProto.Tcp,
            NoDelay,
            Convert.ToUInt32(enable),
            Unsafe.SizeOf<int>());

        if (result == -1)
            Sys.Throw(ExceptionMessages.TcpNoDelay);
    }
}
