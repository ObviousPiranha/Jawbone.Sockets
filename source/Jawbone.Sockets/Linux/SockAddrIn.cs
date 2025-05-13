using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Linux;

struct SockAddrIn
{
    [InlineArray(8)]
    public struct Zero
    {
        private byte _e0;
    }

    public ushort SinFamily;
    public ushort SinPort;
    public uint SinAddr;
    public Zero SinZero;

    public readonly IpEndpoint<IpAddressV4> ToEndpoint()
    {
        if (SinFamily != Af.INet)
            ThrowExceptionFor.WrongAddressFamily();
        return IpEndpoint.Create(
            new IpAddressV4(SinAddr),
            new NetworkPort { NetworkValue = SinPort });
    }

    public static uint Len => Sys.SockLen<SockAddrIn>();

    public static SockAddrIn FromEndpoint(IpEndpoint<IpAddressV4> endpoint)
    {
        return new SockAddrIn
        {
            SinFamily = Af.INet,
            SinPort = endpoint.Port.NetworkValue,
            SinAddr = endpoint.Address.DataU32
        };
    }
}
