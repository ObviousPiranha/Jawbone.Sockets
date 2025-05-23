using System;

namespace Jawbone.Sockets.Test;

public class IpNetworkTest
{
    [Fact]
    public void Network_V4_24_WithInvalidInput_Throws()
    {
        var address = new IpAddressV4 { DataU32 = uint.MaxValue };
        Assert.Throws<ArgumentException>(() => IpNetwork.Create(address, 24));
    }

    [Fact]
    public void Network_V6_120_WithInvalidInput_Throws()
    {
        var address = new IpAddressV6();
        address.DataU32[..].Fill(uint.MaxValue);
        Assert.Throws<ArgumentException>(() => IpNetwork.Create(address, 120));
    }

    [Fact]
    public void Network_V4_24_ContainsAllValidValues()
    {
        byte a = 198;
        byte b = 51;
        byte c = 100;
        var address = new IpAddressV4(a, b, c, 0);
        var network = IpNetwork.Create(address, 24);

        for (int i = 0; i <= byte.MaxValue; ++i)
        {
            address.DataU8[3] = checked((byte)i);
            Assert.True(network.Contains(address));
        }

        address.DataU8[2] += 1;
        address.DataU8[3] = byte.MinValue;
        Assert.False(network.Contains(address));

        address.DataU8[2] -= 2;
        address.DataU8[3] = byte.MaxValue;
        Assert.False(network.Contains(address));
    }

    [Fact]
    public void Network_V6_120_ContainsAllValidValues()
    {
        var address = new IpAddressV6();
        address.DataU8[..^1].Fill(0xab);
        var network = IpNetwork.Create(address, 120);

        for (int i = 0; i <= byte.MaxValue; ++i)
        {
            address.DataU8[^1] = checked((byte)i);
            Assert.True(network.Contains(address));
        }

        address.DataU8[^2] += 1;
        address.DataU8[^1] = byte.MinValue;
        Assert.False(network.Contains(address));

        address.DataU8[^2] -= 2;
        address.DataU8[^1] = byte.MaxValue;
        Assert.False(network.Contains(address));
    }
}