using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jawbone.Sockets.Test;

public class IpEndpointV6Test
{
    [Fact]
    public void Invariants()
    {
        AssertSize<IpEndpoint<IpAddressV6>>(24);

        static void AssertSize<T>(int size) where T : unmanaged
        {
            Assert.Equal(size, Unsafe.SizeOf<T>());
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void ParseUtf16_Matches(
        IpEndpoint<IpAddressV6> expectedEndpoint,
        string expectedUtf16)
    {
        {
            var actualEndpoint = IpEndpoint<IpAddressV6>.Parse(expectedUtf16);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            var actualEndpoint = IpEndpoint<IpAddressV6>.Parse(expectedUtf16, default);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void ParseUtf8_Matches(
        IpEndpoint<IpAddressV6> expectedEndpoint,
        string expectedUtf16)
    {
        var expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            var actualEndpoint = IpEndpoint<IpAddressV6>.Parse(expectedUtf8);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            var actualEndpoint = IpEndpoint<IpAddressV6>.Parse(expectedUtf8, default);
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryParseUtf16_Matches(
        IpEndpoint<IpAddressV6> expectedEndpoint,
        string expectedUtf16)
    {
        {
            Assert.True(IpEndpoint<IpAddressV6>.TryParse(expectedUtf16, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            Assert.True(IpEndpoint<IpAddressV6>.TryParse(expectedUtf16, default, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryParseUtf8_Matches(
        IpEndpoint<IpAddressV6> expectedEndpoint,
        string expectedUtf16)
    {
        ReadOnlySpan<byte> expectedUtf8 = Encoding.UTF8.GetBytes(expectedUtf16);

        {
            Assert.True(IpEndpoint<IpAddressV6>.TryParse(expectedUtf8, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }

        {
            Assert.True(IpEndpoint<IpAddressV6>.TryParse(expectedUtf8, default, out var actualEndpoint));
            Assert.Equal(expectedEndpoint, actualEndpoint);
        }
    }

    [Theory]
    [MemberData(nameof(Endpoints))]
    public void TryFormatUtf16_Matches(
        IpEndpoint<IpAddressV6> expectedEndpoint,
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
        IpEndpoint<IpAddressV6> expectedEndpoint,
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
        IpEndpoint<IpAddressV6> expectedEndpoint,
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

    public static TheoryData<IpEndpoint<IpAddressV6>, string> Endpoints => new()
    {
        { IpAddressV6.Any.OnPort(0), "[::]:0" },
        { IpAddressV6.Local.OnPort(80), "[::1]:80" },
        { IpAddressV6.FromHostU16([0x2001, 0xdb8, 0x4006, 0x812], [0x200e]).OnPort(7777), "[2001:db8:4006:812::200e]:7777" },
        { IpAddressV6.FromHostU16([0x2001, 0xdb8, 0x4006, 0x812], [0x200e], 35).OnPort(25555), "[2001:db8:4006:812::200e%35]:25555" },
        { new IpAddressV6(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15).OnPort(443), "[1:203:405:607:809:a0b:c0d:e0f]:443" },
        { IpAddressV6.FromHostU32(0x01234567, 0x89abcdef, scopeId: 1).OnPort(25), "[123:4567:89ab:cdef::%1]:25" },
        { IpAddressV6.FromHostU32(0x01234567, v3: 0x89abcdef, scopeId: 2).OnPort(9000), "[123:4567::89ab:cdef%2]:9000" },
        { IpAddressV6.FromHostU16([0xfe80], scopeId: 7).OnPort(1), "[fe80::%7]:1" },
        { IpAddressV6.FromHostU16([0xfe80], [0xa, 0xe21]).OnPort(12000), "[fe80::a:e21]:12000" },
        { IpAddressV6.FromHostU64(0x0123456789abcdef, 0xfedcba9876543210).OnPort(38245), "[123:4567:89ab:cdef:fedc:ba98:7654:3210]:38245" },
        { IpAddressV6.FromHostU128(new(0x0123456789abcdef, 0xfedcba9876543210)).OnPort(55), "[123:4567:89ab:cdef:fedc:ba98:7654:3210]:55" },
        { IpAddressV6.FromHostU128(new(0x0123456789abcdef, 0x22446688aaccee)).OnPort(2000), "[123:4567:89ab:cdef:22:4466:88aa:ccee]:2000" },
        { ((IpAddressV6)IpAddressV4.Local).OnPort(22), "[::ffff:127.0.0.1]:22" }
    };
}
