using System;
using System.Text;

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

    [Theory]
    [MemberData(nameof(Networks))]
    public void TryParseUtf16_Matches(
        IpNetwork<IpAddressV4> expectedNetwork,
        string expectedUtf16)
    {
        {
            Assert.True(IpNetwork<IpAddressV4>.TryParse(
                expectedUtf16,
                default,
                out var actualNetwork));
            Assert.Equal(expectedNetwork, actualNetwork);
        }

        {
            Assert.True(IpNetwork<IpAddressV4>.TryParse(
                expectedUtf16,
                out var actualNetwork));
            Assert.Equal(expectedNetwork, actualNetwork);
        }
    }

    [Theory]
    [MemberData(nameof(Networks))]
    public void ParseUtf8_Matches(
        IpNetwork<IpAddressV4> expectedNetwork,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);
        var actualNetwork = IpNetwork<IpAddressV4>.Parse(expectedUtf8);
        Assert.Equal(expectedNetwork, actualNetwork);
    }

    [Theory]
    [MemberData(nameof(Networks))]
    public void TryParseUtf8_Matches(
        IpNetwork<IpAddressV4> expectedNetwork,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);
        {
            Assert.True(IpNetwork<IpAddressV4>.TryParse(
                expectedUtf8,
                default,
                out var actualNetwork));
            Assert.Equal(expectedNetwork, actualNetwork);
        }

        {
            Assert.True(IpNetwork<IpAddressV4>.TryParse(
                expectedUtf8,
                out var actualNetwork));
            Assert.Equal(expectedNetwork, actualNetwork);
        }
    }

    [Theory]
    [MemberData(nameof(Networks))]
    public void ToString_Matches(
        IpNetwork<IpAddressV4> expectedNetwork,
        string expectedUtf16)
    {
        {
            var actualUtf16 = expectedNetwork.ToString();
            Assert.Equal(expectedUtf16, actualUtf16);
        }

        {
            var actualUtf16 = expectedNetwork.ToString(default, default);
            Assert.Equal(expectedUtf16, actualUtf16);
        }
    }

    [Fact]
    public void CreateNetwork_WithOutOfRangePrefix_Throws()
    {
        IpNetworkTest.AssertValuesOutOfRangeThrow<IpAddressV4>();
    }

    [Theory]
    [MemberData(nameof(InvalidPrefixes))]
    public void CreateNetwork_WithInvalidPrefix_ThrowsArgumentException(IpAddressV4 ipAddress, int prefixLength)
    {
        Assert.Throws<ArgumentException>(() => IpNetwork.Create(ipAddress, prefixLength));
        Assert.Throws<ArgumentException>(() => IpAddressV4.CreateNetwork(ipAddress, prefixLength));
    }

    [Theory]
    [MemberData(nameof(InvalidPrefixes))]
    public void TryCreateNetwork_WithInvalidPrefix_ReturnsFalse(IpAddressV4 ipAddress, int prefixLength)
    {
        Assert.False(IpAddressV4.TryCreateNetwork(ipAddress, prefixLength, out _));
        Assert.False(IpAddress.TryCreateNetwork(ipAddress, prefixLength, out _));
        Assert.False(IpNetwork.TryCreate(ipAddress, prefixLength, out _));
    }

    public static TheoryData<IpNetwork<IpAddressV4>, string> Networks => new()
    {
        { new IpAddressV4(198, 51, 100, 0).WithPrefix(24), "198.51.100.0/24" },
        { IpAddressV4.Local.WithPrefix(32), "127.0.0.1/32" },
        { new IpAddressV4().WithPrefix(0), "0.0.0.0/0" }
    };

    public static TheoryData<IpAddressV4, int> InvalidPrefixes => new()
    {
        { new IpAddressV4(128, 0, 0, 0), 0 },
        { new IpAddressV4(255, 255, 0, 0), 8 }
    };
}
