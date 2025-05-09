using System.Runtime.InteropServices;

namespace Piranha.Sockets.Mac;

[StructLayout(LayoutKind.Explicit, Size = 128)]
struct SockAddrStorage
{
    [FieldOffset(0)]
    public SockAddrIn V4;
    [FieldOffset(0)]
    public SockAddrIn6 V6;

    public readonly Endpoint<AddressV4> GetV4(uint addrLen)
    {
        if (addrLen == SockAddrIn.Len)
            return V4.ToEndpoint();
        else if (addrLen == SockAddrIn6.Len)
            return Endpoint.ConvertToV4(V6.ToEndpoint());
        else
            throw CreateExceptionFor.InvalidAddressSize(addrLen);
    }

    public readonly Endpoint<AddressV6> GetV6(uint addrLen)
    {
        if (addrLen == SockAddrIn6.Len)
            return V6.ToEndpoint();
        else if (addrLen == SockAddrIn.Len)
            return V4.ToEndpoint().MapToV6();
        else
            throw CreateExceptionFor.InvalidAddressSize(addrLen);
    }

    public static uint Len => Sys.SockLen<SockAddrStorage>();
}
