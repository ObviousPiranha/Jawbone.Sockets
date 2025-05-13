namespace Jawbone.Sockets.Linux;

struct SockAddrIn6
{
    public ushort Sin6Family;
    public ushort Sin6Port;
    public uint Sin6FlowInfo;
    public In6Addr Sin6Addr;
    public uint Sin6ScopeId;

    public readonly IpEndpoint<IpAddressV6> ToEndpoint()
    {
        if (Sin6Family != Af.INet6)
            ThrowExceptionFor.WrongAddressFamily();
        return IpEndpoint.Create(
            new IpAddressV6(Sin6Addr.U6Addr32, Sin6ScopeId),
            new NetworkPort { NetworkValue = Sin6Port });
    }

    public static uint Len => Sys.SockLen<SockAddrIn6>();

    public static SockAddrIn6 FromEndpoint(IpEndpoint<IpAddressV6> endpoint)
    {
        return new SockAddrIn6
        {
            Sin6Family = Af.INet6,
            Sin6Port = endpoint.Port.NetworkValue,
            Sin6Addr = new(endpoint.Address.DataU32),
            Sin6ScopeId = endpoint.Address.ScopeId
        };
    }
}
