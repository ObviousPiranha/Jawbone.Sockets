namespace Piranha.Sockets.Mac;

// /Library/Developer/CommandLineTools/SDKs/MacOSX.sdk/usr/include/netinet/tcp.h
static class Tcp
{
    public const int NoDelay = 1;

    public static void SetNoDelay(int fd)
    {
        var result = Sys.SetSockOpt(
            fd,
            IpProto.Tcp,
            NoDelay,
            1,
            Sys.SockLen<int>());

        if (result == -1)
            Sys.Throw(ExceptionMessages.TcpNoDelay);
    }
}
