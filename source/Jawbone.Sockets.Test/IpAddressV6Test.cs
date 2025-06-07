using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jawbone.Sockets.Test;

public class IpAddressV6Test
{
    [Fact]
    public void Invariants()
    {
        AssertSize<IpAddressV6>(20);
        AssertSize<IpEndpoint<IpAddressV6>>(24);
        Assert.Equal(IpAddressVersion.V6, IpAddressV6.Version);
        Assert.True(IpAddressV6.Local.IsLoopback);

        {
            var address = default(IpAddressV6);
            byte v = 0xab;
            address.DataU8[..].Fill(v);
            foreach (var b in address.DataU8)
                Assert.Equal(b, v);
        }

        static void AssertSize<T>(int size) where T : unmanaged
        {
            Assert.Equal(size, Unsafe.SizeOf<T>());
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void ParseUtf16_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        {
            var actualAddress = IpAddressV6.Parse(expectedUtf16);
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            var actualAddress = IpAddressV6.Parse(expectedUtf16, default);
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void ParseUtf8_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            var actualAddress = IpAddressV6.Parse(expectedUtf8);
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            var actualAddress = IpAddressV6.Parse(expectedUtf8, default);
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryParseUtf16_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        {
            Assert.True(IpAddressV6.TryParse(expectedUtf16, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            Assert.True(IpAddressV6.TryParse(expectedUtf16, default, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryParseUtf8_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            Assert.True(IpAddressV6.TryParse(expectedUtf8, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            Assert.True(IpAddressV6.TryParse(expectedUtf8, default, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryFormatUtf16_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        Span<char> buffer = new char[64];

        {
            Assert.True(expectedAddress.TryFormat(buffer, out var length));
            Assert.Equal(buffer[..length], expectedUtf16);
        }

        buffer.Clear();

        {
            Assert.True(expectedAddress.TryFormat(buffer, out var length, default, default));
            Assert.Equal(buffer[..length], expectedUtf16);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryFormatUtf8_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        Span<byte> buffer = new byte[64];
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            Assert.True(expectedAddress.TryFormat(buffer, out var length));
            Assert.Equal(buffer[..length], expectedUtf8);
        }

        buffer.Clear();

        {
            Assert.True(expectedAddress.TryFormat(buffer, out var length, default, default));
            Assert.Equal(buffer[..length], expectedUtf8);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void ToString_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        {
            var actualUtf16 = expectedAddress.ToString();
            Assert.Equal(expectedUtf16, actualUtf16);
        }

        {
            var actualUtf16 = expectedAddress.ToString(default, default);
            Assert.Equal(expectedUtf16, actualUtf16);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void DnsQuery_Matches(
        IpAddressV6 expectedAddress,
        string expectedUtf16)
    {
        var actualAddress = Dns.GetAddressV6(expectedUtf16);
        Assert.Equal(expectedAddress, actualAddress);
    }

    [Theory]
    [InlineData("")]
    [InlineData("::g")]
    [InlineData("127.0.0.1")]
    [InlineData("a3b:d:914fc:1::2")]
    [InlineData(" ::")]
    [InlineData("[::")]
    [InlineData("::]")]
    [InlineData("::1 ")]
    public void InvalidInput_FailsParsing(string utf16)
    {
        Assert.False(IpAddressV6.TryParse(utf16, out var result));
        Assert.False(IpAddressV6.TryParse(utf16, null, out result));
        Assert.False(IpAddress.TryParse(utf16, out var result2) && result2.Version == IpAddressVersion.V6);
        Assert.False(IpAddress.TryParse(utf16, null, out result2) && result2.Version == IpAddressVersion.V6);
        Assert.True(result.IsDefault);
        Assert.False(IpAddressV6.TryParse(utf16.AsSpan(), out result));
        Assert.False(IpAddressV6.TryParse(utf16.AsSpan(), null, out result));
        Assert.False(IpAddress.TryParse(utf16.AsSpan(), out result2) && result2.Version == IpAddressVersion.V6);
        Assert.False(IpAddress.TryParse(utf16.AsSpan(), null, out result2) && result2.Version == IpAddressVersion.V6);
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf16));
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf16, null));
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf16.AsSpan()));
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf16.AsSpan(), null));

        if (result2.Version == IpAddressVersion.None)
        {
            Assert.Throws<FormatException>(() => IpAddress.Parse(utf16));
            Assert.Throws<FormatException>(() => IpAddress.Parse(utf16.AsSpan()));
        }

        var utf8 = Encoding.UTF8.GetBytes(utf16);

        Assert.False(IpAddressV6.TryParse(utf8, out result));
        Assert.False(IpAddressV6.TryParse(utf8, null, out result));
        Assert.False(IpAddress.TryParse(utf8, out result2) && result2.Version == IpAddressVersion.V6);
        Assert.False(IpAddress.TryParse(utf8, null, out result2) && result2.Version == IpAddressVersion.V6);
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf8));
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(utf8, null));
        
        if (result2.Version == IpAddressVersion.None)
            Assert.Throws<FormatException>(() => IpAddress.Parse(utf8));
    }

    [Theory]
    [InlineData("0001::0001", "1::1")]
    [InlineData("1:0:0:0:0:0:0:2", "1::2")]
    [InlineData("1::2:0:0:0:0:3", "1:0:2::3")]
    [InlineData("1:0000:2:0:0:0:0:3", "1:0:2::3")]
    [InlineData("1:0:0:0:0:a::3", "1::a:0:3")]
    [InlineData("2001:DB8:4006:812::200E", "2001:db8:4006:812::200e")]
    [InlineData("[::]", "::")]
    [InlineData("::ffff:7f00:1", "::ffff:127.0.0.1")]
    public void UnusualFormat_ParsesSuccessfully(string input, string expectedUtf16)
    {
        Assert.NotEqual(expectedUtf16, input);
        var address = IpAddressV6.Parse(input);
        var actualUtf16 = address.ToString();
        Assert.Equal(expectedUtf16, actualUtf16);
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void CastsToAndFromDotNetIpAddress(IpAddressV6 expected, string _)
    {
        var dotNetIpAddress = (IPAddress)expected;
        {
            var actual = (IpAddressV6)dotNetIpAddress;
            Assert.Equal(expected, actual);
        }

        {
            IpAddress actual = dotNetIpAddress;
            Assert.True(expected == actual);
            Assert.False(expected != actual);
            Assert.True(actual == expected);
            Assert.False(actual != expected);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void DotNetIpAddressFailsCastToV4(IpAddressV6 ipAddress, string _)
    {
        var dotNetIpAddress = (IPAddress)ipAddress;
        Assert.Throws<InvalidCastException>(() => (IpAddressV4)dotNetIpAddress);
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void IpNetworkOfAddressContainsAddress(IpAddressV6 ipAddress, string _)
    {
        var ipNetwork = IpNetwork.Create(ipAddress, IpAddressV6.MaxPrefixLength);
        Assert.True(ipNetwork.Contains(ipAddress));
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void EmptyIpNetworkContainsAddress(IpAddressV6 ipAddress, string _)
    {
        Assert.True(default(IpNetwork<IpAddressV6>).Contains(ipAddress));
    }

    public static TheoryData<IpAddressV6, string> Addresses => new()
    {
        { IpAddressV6.Any, "::" },
        { IpAddressV6.Local, "::1" },
        { IpAddressV6.FromHostU16([0x2001, 0xdb8, 0x4006, 0x812], [0x200e]), "2001:db8:4006:812::200e" },
        { IpAddressV6.FromHostU16([0x2001, 0xdb8, 0x4006, 0x812], [0x200e], 35), "2001:db8:4006:812::200e%35" },
        { IpAddressV6.FromBytes(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15), "1:203:405:607:809:a0b:c0d:e0f" },
        { IpAddressV6.FromHostU32([0x01234567, 0x89abcdef], scopeId: 1), "123:4567:89ab:cdef::%1" },
        { IpAddressV6.FromHostU32([0x01234567], [0x89abcdef], 2), "123:4567::89ab:cdef%2" },
        { IpAddressV6.FromHostU16([0xfe80], scopeId: 7), "fe80::%7" },
        { IpAddressV6.FromHostU16([0xfe80], [0xa, 0xe21]), "fe80::a:e21" },
        { IpAddressV6.FromHostU64(0x0123456789abcdef, 0xfedcba9876543210), "123:4567:89ab:cdef:fedc:ba98:7654:3210" },
        { IpAddressV6.FromHostU128(new(0x0123456789abcdef, 0xfedcba9876543210)), "123:4567:89ab:cdef:fedc:ba98:7654:3210" },
        { IpAddressV6.FromHostU128(new(0x0123456789abcdef, 0x22446688aaccee)), "123:4567:89ab:cdef:22:4466:88aa:ccee" },
        { (IpAddressV6)IpAddressV4.Local, "::ffff:127.0.0.1" }
    };
}
