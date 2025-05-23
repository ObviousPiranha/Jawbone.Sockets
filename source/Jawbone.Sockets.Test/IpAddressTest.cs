using System;
using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Test;

file static class Extensions
{
    public static IpAddressV6 WithScopeId(this IpAddressV6 address, uint scopeId)
    {
        address.ScopeId = scopeId;
        return address;
    }
}

public class IpAddressTest
{
    [Fact]
    public void AddressInvariants()
    {
        AssertSize<NetworkPort>(2);
        AssertSize<IpAddressV4>(4);
        AssertSize<IpAddressV6>(20);
        AssertSize<IpEndpoint<IpAddressV4>>(8);
        AssertSize<IpEndpoint<IpAddressV6>>(24);

        Assert.True(IpAddressV4.Local.IsLoopback);
        Assert.True(IpAddressV6.Local.IsLoopback);
        var debug = IpAddressV6.Local.ToString();

        Assert.Throws<ArgumentNullException>(() => IpAddressV4.Parse(default(string)!, null));
        Assert.Throws<ArgumentNullException>(() => IpAddressV6.Parse(default(string)!, null));

        static void AssertSize<T>(int size) where T : unmanaged
        {
            Assert.Equal(size, Unsafe.SizeOf<T>());
        }
    }

    [Theory]
    [MemberData(nameof(LinkLocal32))]
    public void AddressV4_IsLinkLocal(Serializable<IpAddressV4> address)
    {
        Assert.True(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(NotLinkLocal32))]
    public void AddressV4_IsNotLinkLocal(Serializable<IpAddressV4> address)
    {
        Assert.False(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(LinkLocal128))]
    public void AddressV6_IsLinkLocal(Serializable<IpAddressV6> address)
    {
        Assert.True(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(NotLinkLocal128))]
    public void AddressV6_IsNotLinkLocal(Serializable<IpAddressV6> address)
    {
        Assert.False(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripParseString(Serializable<IpAddressV4> expected)
    {
        var asString = expected.ToString();
        var actual = IpAddressV4.Parse(asString, null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripParseSpan(Serializable<IpAddressV4> expected)
    {
        var asString = expected.ToString();
        var actual = IpAddressV4.Parse(asString.AsSpan(), null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripTryParseString(Serializable<IpAddressV4> expected)
    {
        var asString = expected.ToString();
        Assert.True(IpAddressV4.TryParse(asString, null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripTryParseSpan(Serializable<IpAddressV4> expected)
    {
        var asString = expected.ToString();
        Assert.True(IpAddressV4.TryParse(asString.AsSpan(), null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripParseUtf8Span(Serializable<IpAddressV4> expected)
    {
        Span<byte> buffer = new byte[64];
        Assert.True(expected.Value.TryFormat(buffer, out var n));
        var actual = IpAddressV4.Parse(buffer[..n]);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV4))]
    public void AddressV4_RoundTripTryParseUtf8Span(Serializable<IpAddressV4> expected)
    {
        Span<byte> buffer = new byte[64];
        Assert.True(expected.Value.TryFormat(buffer, out var n));
        Assert.True(IpAddressV4.TryParse(buffer[..n], out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("asdf")]
    [InlineData("127.0.0.")]
    [InlineData("1,1,1,1")]
    [InlineData("253.254.255.256")]
    public void AddressV4_FailsParsing(string s)
    {
        Assert.False(IpAddressV4.TryParse(s, null, out var result));
        Assert.True(result.IsDefault);
        Assert.False(IpAddressV4.TryParse(s.AsSpan(), null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(s, null));
        Assert.Throws<FormatException>(() => IpAddressV4.Parse(s.AsSpan(), null));
    }

    [Theory]
    [InlineData("127.000.000.001", "127.0.0.1")]
    public void AddressV4_UnusualFormat_ParsesSuccessfully(string input, string expectedCanonical)
    {
        var address = IpAddressV4.Parse(input);
        var actualCanonical = address.ToString();
        Assert.Equal(expectedCanonical, actualCanonical);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripParseString(Serializable<IpAddressV6> expected)
    {
        var asString = expected.ToString();
        var actual = IpAddressV6.Parse(asString, null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripParseSpan(Serializable<IpAddressV6> expected)
    {
        var asString = expected.ToString();
        var actual = IpAddressV6.Parse(asString.AsSpan(), null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripTryParseString(Serializable<IpAddressV6> expected)
    {
        var asString = expected.ToString();
        Assert.True(IpAddressV6.TryParse(asString, null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripTryParseSpan(Serializable<IpAddressV6> expected)
    {
        var asString = expected.ToString();
        Assert.True(IpAddressV6.TryParse(asString.AsSpan(), null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripParseUtf8Span(Serializable<IpAddressV6> expected)
    {
        Span<byte> buffer = new byte[64];
        Assert.True(expected.Value.TryFormat(buffer, out var n));
        var actual = IpAddressV6.Parse(buffer[..n]);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParseV6))]
    public void AddressV6_RoundTripTryParseUtf8Span(Serializable<IpAddressV6> expected)
    {
        Span<byte> buffer = new byte[64];
        Assert.True(expected.Value.TryFormat(buffer, out var n));
        Assert.True(IpAddressV6.TryParse(buffer[..n], out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(":::")]
    [InlineData("c3ee:abce:12345::2")]
    [InlineData("cf:0::71ef:91e::6")]
    public void AddressV6_FailsParsing(string s)
    {
        Assert.False(IpAddressV6.TryParse(s, null, out var result));
        Assert.True(result.IsDefault);
        Assert.False(IpAddressV6.TryParse(s.AsSpan(), null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(s, null));
        Assert.Throws<FormatException>(() => IpAddressV6.Parse(s.AsSpan(), null));
    }

    [Theory]
    [InlineData("0001::0001", "1::1")]
    [InlineData("1:0:0:0:0:0:0:2", "1::2")]
    [InlineData("1::2:0:0:0:0:3", "1:0:2::3")]
    [InlineData("1:0000:2:0:0:0:0:3", "1:0:2::3")]
    public void AddressV6_UnusualFormat_ParsesSuccessfully(string input, string expectedCanonical)
    {
        var address = IpAddressV6.Parse(input);
        var actualCanonical = address.ToString();
        Assert.Equal(expectedCanonical, actualCanonical);
    }

    public static TheoryData<Serializable<IpAddressV4>> LinkLocal32 => new()
    {
        new IpAddressV4(169, 254, 0, 0),
        new IpAddressV4(169, 254, 127, 127),
        new IpAddressV4(169, 254, 255, 255)
    };

    public static TheoryData<Serializable<IpAddressV4>> NotLinkLocal32 => new()
    {
        IpAddressV4.Local,
        IpAddressV4.Broadcast,
        new IpAddressV4(169, 200, 0, 0),
        new IpAddressV4(170, 254, 0, 0)
    };

    public static TheoryData<Serializable<IpAddressV6>> LinkLocal128 => new()
    {
        MakeLinkLocal()
    };

    public static TheoryData<Serializable<IpAddressV6>> NotLinkLocal128 => new()
    {
        IpAddressV6.Local,
        Create(static span => span.Fill(0xab))
    };

    public static TheoryData<Serializable<IpAddressV4>> RoundTripParseV4 => new()
    {
        IpAddressV4.Any,
        IpAddressV4.Local,
        IpAddressV4.Broadcast,
        new IpAddressV4(192, 168, 0, 1),
        new IpAddressV4(0, 1, 2, 3),
        new IpAddressV4(10, 0, 0, 1),
        new IpAddressV4(1, 1, 1, 1),
        new IpAddressV4(172, 16, 254, 1)
    };

    public static TheoryData<Serializable<IpAddressV6>> RoundTripParseV6 => new()
    {
        IpAddressV6.Any,
        IpAddressV6.Local,
        Create(static span => span.Fill(0xab)),
        Create(static span =>
        {
            span[3] = 0xb;
            span[11] = 0xce;
        }),
        (IpAddressV6)IpAddressV4.Local,
        (IpAddressV6)IpAddressV4.Broadcast,
        Create(static span => span.Fill(0xab)).WithScopeId(55),
        IpAddressV6.Local.WithScopeId(127)
    };

    private static IpAddressV6 Create(Action<Span<byte>> action)
    {
        var result = default(IpAddressV6);
        action.Invoke(result.DataU8);
        return result;
    }

    private static IpAddressV6 MakeLinkLocal()
    {
        var result = default(IpAddressV6);
        result.DataU8[0] = 0xfe;
        result.DataU8[1] = 0x80;
        return result;
    }
}
