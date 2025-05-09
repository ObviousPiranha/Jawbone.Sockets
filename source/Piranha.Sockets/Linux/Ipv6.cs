namespace Piranha.Sockets.Linux;

static class Ipv6
{
    public const int V6Only = 26;

    public static void SetIpv6Only(int fd, bool allowV4)
    {
        int yes = allowV4 ? 0 : 1;
        var result = Sys.SetSockOpt(
            fd,
            IpProto.Ipv6,
            V6Only,
            yes,
            Sys.SockLen<int>());

        if (result == -1)
            Sys.Throw("Unable to set socket option.");
    }
}
