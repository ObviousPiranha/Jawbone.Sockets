using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Jawbone.Sockets.Test;

public class IpAddressV4Test
{
    [Fact]
    public void Invariants()
    {
        AssertSize<IpAddressV4>(4);
        AssertSize<IpEndpoint<IpAddressV4>>(8);
        Assert.True(IpAddressV4.Local.IsLoopback);

        static void AssertSize<T>(int size) where T : unmanaged
        {
            Assert.Equal(size, Unsafe.SizeOf<T>());
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void ParseUtf16_Matches(
        IpAddressV4 expectedAddress,
        string expectedUtf16)
    {
        {
            var actualAddress = IpAddressV4.Parse(expectedUtf16);
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            var actualAddress = IpAddressV4.Parse(expectedUtf16, default);
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void ParseUtf8_Matches(
        IpAddressV4 expectedAddress,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);
        
        {
            var actualAddress = IpAddressV4.Parse(expectedUtf8);
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            var actualAddress = IpAddressV4.Parse(expectedUtf8, default);
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryParseUtf16_Matches(
        IpAddressV4 expectedAddress,
        string expectedUtf16)
    {
        {
            Assert.True(IpAddressV4.TryParse(expectedUtf16, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            Assert.True(IpAddressV4.TryParse(expectedUtf16, default, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryParseUtf8_Matches(
        IpAddressV4 expectedAddress,
        string expectedUtf16)
    {
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);
        
        {
            Assert.True(IpAddressV4.TryParse(expectedUtf8, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }

        {
            Assert.True(IpAddressV4.TryParse(expectedUtf8, default, out var actualAddress));
            Assert.Equal(expectedAddress, actualAddress);
        }
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void TryFormatUtf16_Matches(
        IpAddressV4 expectedAddress,
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
        IpAddressV4 expectedAddress,
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
        IpAddressV4 expectedAddress,
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
    [InlineData("")]
    [InlineData("asdf")]
    [InlineData(" 127.0.0.1")]
    [InlineData("127.0.0.1 ")]
    [InlineData("127.0.0.")]
    [InlineData("1,1,1,1")]
    [InlineData("253.254.255.256")]
    [InlineData("1.2.3.4.5")]
    public void InvalidInput_FailsParsing(string utf16)
    {
        Assert.False(IpAddressV4.TryParse(utf16, out var result));
        Assert.False(IpAddressV4.TryParse(utf16, null, out result));
        Assert.True(result.IsDefault);
        Assert.False(IpAddressV4.TryParse(utf16.AsSpan(), out result));
        Assert.False(IpAddressV4.TryParse(utf16.AsSpan(), null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf16));
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf16, null));
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf16.AsSpan()));
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf16.AsSpan(), null));

        var utf8 = Encoding.UTF8.GetBytes(utf16);

        Assert.False(IpAddressV4.TryParse(utf8, out result));
        Assert.False(IpAddressV4.TryParse(utf8, null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf8));
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(utf8, null));
    }

    [Theory]
    [InlineData("127.000.000.001", "127.0.0.1")]
    [InlineData("010.000.000.001", "10.0.0.1")]
    public void UnusualFormat_ParsesSuccessfully(string input, string expectedUtf16)
    {
        Assert.NotEqual(expectedUtf16, input);
        var address = IpAddressV4.Parse(input);
        var actualUtf16 = address.ToString();
        Assert.Equal(expectedUtf16, actualUtf16);
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void CastsToAndFromDotNetIpAddress(IpAddressV4 expected, string _)
    {
        var dotNetIpAddress = (IPAddress)expected;
        var actual = (IpAddressV4)dotNetIpAddress;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Addresses))]
    public void DotNetIpAddressFailsCastToV6(IpAddressV4 ipAddress, string _)
    {
        var dotNetIpAddress = (IPAddress)ipAddress;
        Assert.Throws<InvalidCastException>(() => (IpAddressV6)dotNetIpAddress);
    }

    public static TheoryData<IpAddressV4, string> Addresses => new()
    {
        { IpAddressV4.Any, "0.0.0.0" },
        { IpAddressV4.Local, "127.0.0.1" },
        { IpAddressV4.Broadcast, "255.255.255.255" },
        { new IpAddressV4(1, 1, 1, 1), "1.1.1.1" },
        { new IpAddressV4(8, 8, 8, 8), "8.8.8.8" },
        { new IpAddressV4(8, 8, 4, 4), "8.8.4.4" },
        { new IpAddressV4(192, 168, 0, 1), "192.168.0.1" },
        { new IpAddressV4(10, 0, 0, 1), "10.0.0.1" },
        { new IpAddressV4(3, 97, 111, 4), "3.97.111.4" },
    };
}