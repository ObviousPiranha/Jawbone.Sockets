using BenchmarkDotNet.Attributes;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class IpAddressV4ParseBenchmark
{
    private const string Input = "192.168.0.1";

    [Benchmark(Baseline = true)]
    public IpAddressV4 JawboneParse() => IpAddressV4.Parse(Input);

    [Benchmark]
    public IPAddress DotNetParse() => IPAddress.Parse(Input);
}