using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Windows;

static class Ipv6
{
    public const int V6Only = 27;

    public static void SetIpv6Only(nuint socket, bool allowV4)
    {
        uint yes = allowV4 ? 0u : 1u;
        var result = Sys.SetSockOpt(
        socket,
        IpProto.Ipv6,
        V6Only,
        yes,
        Unsafe.SizeOf<uint>());

        if (result == -1)
            Sys.Throw("Unable to set socket option.");
    }
}
