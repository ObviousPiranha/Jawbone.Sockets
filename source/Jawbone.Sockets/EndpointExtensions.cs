namespace Jawbone.Sockets;

public static class EndpointExtensions
{
    public static Endpoint<AddressV6> MapToV6(this Endpoint<AddressV4> endpoint)
    {
        return Endpoint.Create((AddressV6)endpoint.Address, endpoint.Port);
    }
}
