using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets.Windows;

[StructLayout(LayoutKind.Explicit, Size = 128)]
struct SockAddrStorage
{
    [FieldOffset(0)]
    public SockAddrIn V4;
    [FieldOffset(0)]
    public SockAddrIn6 V6;

    public readonly IpEndpoint<IpAddressV4> GetV4(int addrLen)
    {
        if (addrLen == SockAddrIn.Len)
            return V4.ToEndpoint();
        else if (addrLen == SockAddrIn6.Len)
            return IpEndpoint.ConvertToV4(V6.ToEndpoint());
        else
            throw CreateExceptionFor.InvalidAddressSize(addrLen);
    }

    public readonly IpEndpoint<IpAddressV6> GetV6(int addrLen)
    {
        if (addrLen == SockAddrIn6.Len)
            return V6.ToEndpoint();
        else if (addrLen == SockAddrIn.Len)
            return IpEndpoint.ConvertToV6(V4.ToEndpoint());
        else
            throw CreateExceptionFor.InvalidAddressSize(addrLen);
    }

    public static int Len => Unsafe.SizeOf<SockAddrStorage>();
}
