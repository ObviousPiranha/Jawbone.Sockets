using System.Linq;

namespace Jawbone.Sockets.Test;

public class DnsTest
{
    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("10.0.0.23")]
    [InlineData("255.255.255.255")]
    [InlineData("171.56.9.111")]
    public void ParseAddressV4MatchesDnsQuery(string address)
    {
        var parsed = IpAddressV4.Parse(address, null);
        var queried = Dns.QueryV4(address).First().Address;
        Assert.Equal(parsed, queried);
    }

    [Theory]
    [InlineData("::")]
    [InlineData("::1")]
    [InlineData("0000:0000:0000:0000:0000:0000:0000:0001")]
    [InlineData("2001:db8:0:0:0:ff00:42:8329")]
    public void ParseAddressV6MatchesDnsQuery(string address)
    {
        var parsed = IpAddressV6.Parse(address, null);
        var queried = Dns.QueryV6(address).First().Address;
        Assert.Equal(parsed, queried);
    }
}