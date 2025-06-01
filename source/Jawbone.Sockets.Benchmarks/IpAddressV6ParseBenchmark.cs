using BenchmarkDotNet.Attributes;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class IpAddressV6ParseBenchmark
{
    private const string Input = "f65e:2f13:10d0:8fd8:67c:16ee:fe80:b235";

    [Benchmark(Baseline = true)]
    public IpAddressV6 JawboneParse() => IpAddressV6.Parse(Input);

    [Benchmark]
    public IPAddress DotNetParse() => IPAddress.Parse(Input);
}
