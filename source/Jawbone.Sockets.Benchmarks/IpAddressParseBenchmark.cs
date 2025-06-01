using BenchmarkDotNet.Attributes;
using System;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class IpAddressParseBenchmark
{
    private const string Utf16InputV4 = "192.168.0.1";
    private static ReadOnlySpan<byte> Utf8InputV4 => "192.168.0.1"u8;
    private const string Utf16InputV6 = "f65e:2f13:10d0:8fd8:67c:16ee:fe80:b235";
    private static ReadOnlySpan<byte> Utf8InputV6 => "f65e:2f13:10d0:8fd8:67c:16ee:fe80:b235"u8;

    [Benchmark]
    public IpAddressV4 JawboneUtf16ParseV4() => IpAddressV4.Parse(Utf16InputV4);

    [Benchmark]
    public IpAddressV4 JawboneUtf8ParseV4() => IpAddressV4.Parse(Utf8InputV4);

    [Benchmark]
    public bool JawboneUtf16TryParseV4() => IpAddressV4.TryParse(Utf16InputV4, out _);

    [Benchmark]
    public bool JawboneUtf8TryParseV4() => IpAddressV4.TryParse(Utf8InputV4, out _);

    [Benchmark]
    public IpAddressV6 JawboneUtf16ParseV6() => IpAddressV6.Parse(Utf16InputV6);

    [Benchmark]
    public IpAddressV6 JawboneUtf8ParseV6() => IpAddressV6.Parse(Utf8InputV6);

    [Benchmark]
    public bool JawboneUtf16TryParseV6() => IpAddressV6.TryParse(Utf16InputV6, out _);

    [Benchmark]
    public bool JawboneUtf8TryParseV6() => IpAddressV6.TryParse(Utf8InputV6, out _);

    // System.Net.Sockets

    [Benchmark]
    public IPAddress DotNetUtf16ParseV4() => IPAddress.Parse(Utf16InputV4);

    [Benchmark]
    public bool DotNetUtf16TryParseV4() => IPAddress.TryParse(Utf16InputV4, out _);

    [Benchmark]
    public IPAddress DotNetUtf16ParseV6() => IPAddress.Parse(Utf16InputV6);

    [Benchmark]
    public bool DotNetUtf16TryParseV6() => IPAddress.TryParse(Utf16InputV6, out _);
}
