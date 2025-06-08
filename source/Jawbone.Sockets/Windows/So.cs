using System;
using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Windows;

public static class So
{
    public const int ReuseAddr = 4;

    public static void SetReuseAddr(nuint fd, bool enable)
    {
        var result = Sys.SetSockOpt(
            fd,
            Sol.Socket,
            ReuseAddr,
            Convert.ToUInt32(enable),
            Unsafe.SizeOf<uint>());

        if (result == -1)
            Sys.Throw(ExceptionMessages.ReuseAddress);
    }
}
