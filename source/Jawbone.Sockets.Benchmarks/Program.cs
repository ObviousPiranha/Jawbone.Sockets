using BenchmarkDotNet.Running;
using Jawbone.Sockets.Benchmarks;

// BenchmarkRunner.Run<TimeSpanBenchmark>();
// BenchmarkRunner.Run<UdpBenchmark>();
// BenchmarkRunner.Run<IpAddressV4ParseBenchmark>();
// BenchmarkRunner.Run<IpAddressV6ParseBenchmark>();
BenchmarkRunner.Run<IpAddressParseBenchmark>();

