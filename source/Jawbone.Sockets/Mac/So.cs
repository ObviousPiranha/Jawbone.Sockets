using System;

namespace Jawbone.Sockets.Mac;

public static class So
{
    public const int ReuseAddr = 4;
    public const int Broadcast = 32;

    public static void SetReuseAddr(int fd, bool enable)
    {
        var result = Sys.SetSockOpt(
            fd,
            Sol.Socket,
            ReuseAddr,
            Convert.ToInt32(enable),
            Sys.SockLen<int>());

        if (result == -1)
            Sys.Throw(ExceptionMessages.ReuseAddress);
    }

    public static void SetBroadcast(int fd, bool enable)
    {
        var result = Sys.SetSockOpt(
            fd,
            Sol.Socket,
            Broadcast,
            Convert.ToInt32(enable),
            Sys.SockLen<int>());

        if (result == -1)
            Sys.Throw(ExceptionMessages.ReuseAddress);
    }
}
