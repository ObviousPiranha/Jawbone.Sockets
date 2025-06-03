using BenchmarkDotNet.Attributes;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class IpAddressFormatBenchmark
{
    private const string V4 = "192.168.0.1";
    private const string V6 = "f65e:2f13:10d0:8fd8:67c:16ee:fe80:b235";

    private readonly IpAddressV4 _jawboneV4 = IpAddressV4.Parse(V4);
    private readonly IpAddressV6 _jawboneV6 = IpAddressV6.Parse(V6);
    private readonly IPAddress _dotNetV4 = IPAddress.Parse(V4);
    private readonly IPAddress _dotNetV6 = IPAddress.Parse(V6);

    private readonly byte[] _utf8 = new byte[64];
    private readonly char[] _utf16 = new char[64];

    [Benchmark]
    public bool JawboneUtf8V4() => _jawboneV4.TryFormat(_utf8, out _);

    [Benchmark]
    public bool JawboneUtf16V4() => _jawboneV4.TryFormat(_utf16, out _);

    [Benchmark]
    public bool DotNetUtf16V4() => _dotNetV4.TryFormat(_utf16, out _);

    [Benchmark]
    public bool JawboneUtf8V6() => _jawboneV6.TryFormat(_utf8, out _);

    [Benchmark]
    public bool JawboneUtf16V6() => _jawboneV6.TryFormat(_utf16, out _);

    [Benchmark]
    public bool DotNetUtf16V6() => _dotNetV6.TryFormat(_utf16, out _);
}