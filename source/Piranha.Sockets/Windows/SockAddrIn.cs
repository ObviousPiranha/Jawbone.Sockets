using System.Runtime.CompilerServices;

namespace Piranha.Sockets.Windows;

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

    public readonly Endpoint<AddressV4> ToEndpoint()
    {
        if (SinFamily != Af.INet)
            ThrowExceptionFor.WrongAddressFamily();
        return Endpoint.Create(
            new AddressV4(SinAddr),
            new NetworkPort { NetworkValue = SinPort });
    }

    public static int Len => Unsafe.SizeOf<SockAddrIn>();

    public static SockAddrIn FromEndpoint(Endpoint<AddressV4> endpoint)
    {
        return new SockAddrIn
        {
            SinFamily = Af.INet,
            SinPort = endpoint.Port.NetworkValue,
            SinAddr = endpoint.Address.DataU32
        };
    }
}
