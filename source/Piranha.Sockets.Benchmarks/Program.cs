using BenchmarkDotNet.Running;
using System;

namespace Piranha.Sockets.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkRunner.Run<TimeSpanBenchmark>();
    }
}
