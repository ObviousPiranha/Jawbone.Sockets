namespace Piranha.Sockets.Linux;

unsafe struct AddrInfo
{
    public int AiFlags;
    public int AiFamily;
    public int AiSockType;
    public int AiProtocol;
    public uint AiAddrLen;
    public void* AiAddr;
    public nint AiCanonName;
    public AddrInfo* AiNext;
}
