namespace Jawbone.Sockets.Test;

public class IpNetworkV4Test
{
    [Theory]
    [MemberData(nameof(Networks))]
    public void ParseUtf16_Matches(
        IpNetwork<IpAddressV4> expectedNetwork,
        string expectedUtf16)
    {
        var actualNetwork = IpNetwork<IpAddressV4>.Parse(expectedUtf16);
        Assert.Equal(expectedNetwork, actualNetwork);
    }

    public static TheoryData<IpNetwork<IpAddressV4>, string> Networks => new()
    {
        { new IpAddressV4(198, 51, 100, 0).WithPrefix(24), "198.51.100.0/24" }
        // TODO
    };
}