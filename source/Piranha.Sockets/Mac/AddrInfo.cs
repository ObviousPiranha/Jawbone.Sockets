namespace Piranha.Sockets.Mac;

unsafe struct AddrInfo
{
    public int AiFlags;
    public int AiFamily;
    public int AiSockType;
    public int AiProtocol;
    public uint AiAddrLen;
    public nint AiCanonName;
    public void* AiAddr;
    public AddrInfo* AiNext;
}
