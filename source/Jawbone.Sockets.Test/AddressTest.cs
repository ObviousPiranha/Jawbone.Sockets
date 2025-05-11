using System;
using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Test;

file static class Extensions
{
    public static AddressV6 WithScopeId(this AddressV6 address, uint scopeId)
    {
        address.ScopeId = scopeId;
        return address;
    }
}

public class AddressTest
{
    [Fact]
    public void AddressInvariants()
    {
        Assert.Equal(2, Unsafe.SizeOf<NetworkPort>());
        Assert.Equal(4, Unsafe.SizeOf<AddressV4>());
        Assert.Equal(20, Unsafe.SizeOf<AddressV6>());

        Assert.True(AddressV4.Local.IsLoopback);
        Assert.True(AddressV6.Local.IsLoopback);
        var debug = AddressV6.Local.ToString();

        Assert.Throws<ArgumentNullException>(() => AddressV4.Parse(null!, null));
        Assert.Throws<ArgumentNullException>(() => AddressV6.Parse(null!, null));
    }

    [Theory]
    [MemberData(nameof(LinkLocal32))]
    public void AddressV4_IsLinkLocal(Serializable<AddressV4> address)
    {
        Assert.True(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(NotLinkLocal32))]
    public void AddressV4_IsNotLinkLocal(Serializable<AddressV4> address)
    {
        Assert.False(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(LinkLocal128))]
    public void AddressV6_IsLinkLocal(Serializable<AddressV6> address)
    {
        Assert.True(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(NotLinkLocal128))]
    public void AddressV6_IsNotLinkLocal(Serializable<AddressV6> address)
    {
        Assert.False(address.Value.IsLinkLocal);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse32))]
    public void AddressV4_RoundTripParseString(Serializable<AddressV4> expected)
    {
        var asString = expected.ToString();
        var actual = AddressV4.Parse(asString, null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse32))]
    public void AddressV4_RoundTripParseSpan(Serializable<AddressV4> expected)
    {
        var asString = expected.ToString();
        var actual = AddressV4.Parse(asString.AsSpan(), null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse32))]
    public void AddressV4_RoundTripTryParseString(Serializable<AddressV4> expected)
    {
        var asString = expected.ToString();
        Assert.True(AddressV4.TryParse(asString, null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse32))]
    public void AddressV4_RoundTripTryParseSpan(Serializable<AddressV4> expected)
    {
        var asString = expected.ToString();
        Assert.True(AddressV4.TryParse(asString.AsSpan(), null, out var actual));
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
        Assert.False(AddressV4.TryParse(s, null, out var result));
        Assert.True(result.IsDefault);
        Assert.False(AddressV4.TryParse(s.AsSpan(), null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => AddressV4.Parse(s, null));
        Assert.Throws<FormatException>(() => AddressV4.Parse(s.AsSpan(), null));
    }

    [Theory]
    [MemberData(nameof(RoundTripParse128))]
    public void AddressV6_RoundTripParseString(Serializable<AddressV6> expected)
    {
        var asString = expected.ToString();
        var actual = AddressV6.Parse(asString, null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse128))]
    public void AddressV6_RoundTripParseSpan(Serializable<AddressV6> expected)
    {
        var asString = expected.ToString();
        var actual = AddressV6.Parse(asString.AsSpan(), null);
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse128))]
    public void AddressV6_RoundTripTryParseString(Serializable<AddressV6> expected)
    {
        var asString = expected.ToString();
        Assert.True(AddressV6.TryParse(asString, null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [MemberData(nameof(RoundTripParse128))]
    public void AddressV6_RoundTripTryParseSpan(Serializable<AddressV6> expected)
    {
        var asString = expected.ToString();
        Assert.True(AddressV6.TryParse(asString.AsSpan(), null, out var actual));
        Assert.Equal(expected.Value, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(":::")]
    [InlineData("c3ee:abce:12345::2")]
    [InlineData("cf:0::71ef:91e::6")]
    public void AddressV6_FailsParsing(string s)
    {
        Assert.False(AddressV6.TryParse(s, null, out var result));
        Assert.True(result.IsDefault);
        Assert.False(AddressV6.TryParse(s.AsSpan(), null, out result));
        Assert.True(result.IsDefault);
        Assert.Throws<FormatException>(() => AddressV6.Parse(s, null));
        Assert.Throws<FormatException>(() => AddressV6.Parse(s.AsSpan(), null));
    }

    public static TheoryData<Serializable<AddressV4>> LinkLocal32 => new()
    {
        new AddressV4(169, 254, 0, 0),
        new AddressV4(169, 254, 127, 127),
        new AddressV4(169, 254, 255, 255)
    };

    public static TheoryData<Serializable<AddressV4>> NotLinkLocal32 => new()
    {
        AddressV4.Local,
        AddressV4.Broadcast,
        new AddressV4(169, 200, 0, 0),
        new AddressV4(170, 254, 0, 0)
    };

    public static TheoryData<Serializable<AddressV6>> LinkLocal128 => new()
    {
        MakeLinkLocal()
    };

    public static TheoryData<Serializable<AddressV6>> NotLinkLocal128 => new()
    {
        AddressV6.Local,
        Create(static span => span.Fill(0xab))
    };

    public static TheoryData<Serializable<AddressV4>> RoundTripParse32 => new()
    {
        AddressV4.Local,
        AddressV4.Broadcast,
        new AddressV4(192, 168, 0, 1),
        new AddressV4(0, 1, 2, 3)
    };

    public static TheoryData<Serializable<AddressV6>> RoundTripParse128 => new()
    {
        AddressV6.Local,
        Create(static span => span.Fill(0xab)),
        Create(static span =>
        {
            span[3] = 0xb;
            span[11] = 0xce;
        }),
        (AddressV6)AddressV4.Local,
        (AddressV6)AddressV4.Broadcast,
        Create(static span => span.Fill(0xab)).WithScopeId(55),
        AddressV6.Local.WithScopeId(127)
    };

    private static AddressV6 Create(Action<Span<byte>> action)
    {
        var result = default(AddressV6);
        action.Invoke(result.DataU8);
        return result;
    }

    private static AddressV6 MakeLinkLocal()
    {
        var result = default(AddressV6);
        result.DataU8[0] = 0xfe;
        result.DataU8[1] = 0x80;
        return result;
    }
}
