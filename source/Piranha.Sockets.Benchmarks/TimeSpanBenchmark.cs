using BenchmarkDotNet.Attributes;
using System;
using System.Diagnostics;

namespace Piranha.Sockets.Benchmarks;

[MemoryDiagnoser]
public class TimeSpanBenchmark
{
    private readonly long TickStart = Environment.TickCount64;
    private readonly long StopwatchStart = Stopwatch.GetTimestamp();

    [Benchmark(Baseline = true)]
    public TimeSpan UseStopwatch() => Stopwatch.GetElapsedTime(StopwatchStart);

    [Benchmark]
    public TimeSpan UseTick() => new TimeSpan((Environment.TickCount64 - TickStart) * TimeSpan.TicksPerMillisecond);

}
