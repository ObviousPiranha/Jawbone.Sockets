namespace Piranha.Sockets.Windows;

unsafe struct AddrInfo
{
    public int AiFlags;
    public int AiFamily;
    public int AiSockType;
    public int AiProtocol;
    public nuint AiAddrLen;
    public nint AiCanonName;
    public void* AiAddr;
    public AddrInfo* AiNext;
}
