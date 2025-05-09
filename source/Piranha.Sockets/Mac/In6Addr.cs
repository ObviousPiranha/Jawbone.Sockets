using System.Runtime.InteropServices;

namespace Piranha.Sockets.Mac;

[StructLayout(LayoutKind.Explicit, Size = 16)]
struct In6Addr
{
    [FieldOffset(0)]
    public AddressV6.ArrayU8 U6Addr8;
    [FieldOffset(0)]
    public AddressV6.ArrayU16 U6Addr16;
    [FieldOffset(0)]
    public AddressV6.ArrayU32 U6Addr32;

    public In6Addr(AddressV6.ArrayU32 u6addr32) => U6Addr32 = u6addr32;
}
