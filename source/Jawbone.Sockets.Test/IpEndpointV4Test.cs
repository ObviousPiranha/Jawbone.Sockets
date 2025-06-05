using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jawbone.Sockets.Test;

public class IpEndpointV4Test
{
    [Fact]
    public void Invariants()
    {
        AssertSize<IpEndpoint<IpAddressV4>>(8);

        static void AssertSize<T>(int size) where T : unmanaged
        {
            Assert.Equal(size, Unsafe.SizeOf<T>());
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void ParseUtf16_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        {
            var actualEndpoint = IpEndpoint<IpAddressV4>.Parse(expectedUtf16);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            var actualEndpoint = IpEndpoint<IpAddressV4>.Parse(expectedUtf16, default);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void ParseUtf8_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            var actualEndpoint = IpEndpoint<IpAddressV4>.Parse(expectedUtf8);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            var actualEndpoint = IpEndpoint<IpAddressV4>.Parse(expectedUtf8, default);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryParseUtf16_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        {
            Assert.True(IpEndpoint<IpAddressV4>.TryParse(expectedUtf16, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            Assert.True(IpEndpoint<IpAddressV4>.TryParse(expectedUtf16, default, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryParseUtf8_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            Assert.True(IpEndpoint<IpAddressV4>.TryParse(expectedUtf8, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            Assert.True(IpEndpoint<IpAddressV4>.TryParse(expectedUtf8, default, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryFormatUtf16_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        Span<char> buffer = new char[64];

        {
            Assert.True(expectedEndpoint.TryFormat(buffer, out var length));
            Assert.Equal(buffer[..length], expectedUtf16);
        }

        buffer.Clear();

        {
            Assert.True(expectedEndpoint.TryFormat(buffer, out var length, default, default));
            Assert.Equal(buffer[..length], expectedUtf16);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryFormatUtf8_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        Span<byte> buffer = new byte[64];
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            Assert.True(expectedEndpoint.TryFormat(buffer, out var length));
            Assert.Equal(buffer[..length], expectedUtf8);
        }

        buffer.Clear();

        {
            Assert.True(expectedEndpoint.TryFormat(buffer, out var length, default, default));
            Assert.Equal(buffer[..length], expectedUtf8);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void ToString_Matches(
        IpEndpoint<IpAddressV4> expectedEndpoint,
        string expectedUtf16)
    {
        {
            var actualUtf16 = expectedEndpoint.ToString();
            Assert.Equal(expectedUtf16, actualUtf16);
        }

        {
            var actualUtf16 = expectedEndpoint.ToString(default, default);
            Assert.Equal(expectedUtf16, actualUtf16);
        }
    }
    
    public static TheoryData<IpEndpoint<IpAddressV4>, string> Endpoints => new()
    {
        { IpAddressV4.Any.OnPort(0), "0.0.0.0:0" },
        { IpAddressV4.Local.OnPort(80), "127.0.0.1:80" },
        { IpAddressV4.Broadcast.OnPort(2000), "255.255.255.255:2000" },
        { new IpAddressV4(1, 1, 1, 1).OnPort(25), "1.1.1.1:25" },
        { new IpAddressV4(8, 8, 8, 8).OnPort(443), "8.8.8.8:443" },
        { new IpAddressV4(8, 8, 4, 4).OnPort(9000), "8.8.4.4:9000" },
        { new IpAddressV4(192, 168, 0, 1).OnPort(25555), "192.168.0.1:25555" },
        { new IpAddressV4(10, 0, 0, 1).OnPort(7777), "10.0.0.1:7777" },
        { new IpAddressV4(3, 97, 111, 4).OnPort(64000), "3.97.111.4:64000" },
    };
}