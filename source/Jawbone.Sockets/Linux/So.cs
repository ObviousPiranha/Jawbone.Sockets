namespace Jawbone.Sockets.Linux;

public static class So
{
    public const int ReuseAddr = 2;

    public static void SetReuseAddr(int fd)
    {
        var result = Sys.SetSockOpt(
            fd,
            Sol.Socket,
            ReuseAddr,
            1,
            Sys.SockLen<int>());

        if (result == -1)
            Sys.Throw("Unable to enable SO_REUSEADDR.");
    }
}
