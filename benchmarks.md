# Benchmarks

```
BenchmarkDotNet v0.14.0, Ubuntu 25.04 (Plucky Puffin)
AMD Ryzen 7 7700X, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.300
  [Host]     : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.5 (9.0.525.21509), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
```

## IP Address

### Parse

It's worth noting that `IPAddress` does not support parsing UTF-8.

| Method                 | Mean      | Error     | StdDev    | Gen0   | Allocated |
|----------------------- |----------:|----------:|----------:|-------:|----------:|
| JawboneUtf16ParseV4    |  7.712 ns | 0.0430 ns | 0.0402 ns |      - |         - |
| JawboneUtf8ParseV4     |  7.518 ns | 0.0774 ns | 0.0686 ns |      - |         - |
| DotNetUtf16ParseV4     | 16.094 ns | 0.1325 ns | 0.1239 ns | 0.0024 |      40 B |
| JawboneUtf16TryParseV4 |  7.712 ns | 0.0245 ns | 0.0229 ns |      - |         - |
| JawboneUtf8TryParseV4  |  7.381 ns | 0.0221 ns | 0.0207 ns |      - |         - |
| DotNetUtf16TryParseV4  | 15.910 ns | 0.0694 ns | 0.0579 ns | 0.0024 |      40 B |
| JawboneUtf16ParseV6    | 43.007 ns | 0.1897 ns | 0.1774 ns |      - |         - |
| JawboneUtf8ParseV6     | 42.336 ns | 0.3851 ns | 0.3414 ns |      - |         - |
| DotNetUtf16ParseV6     | 78.296 ns | 0.3787 ns | 0.3542 ns | 0.0048 |      80 B |
| JawboneUtf16TryParseV6 | 39.401 ns | 0.2394 ns | 0.1999 ns |      - |         - |
| JawboneUtf8TryParseV6  | 38.026 ns | 0.2252 ns | 0.2107 ns |      - |         - |
| DotNetUtf16TryParseV6  | 78.887 ns | 0.5180 ns | 0.4846 ns | 0.0048 |      80 B |

## UDP

### Send-Poll-Receive

| Method            | Mean     | Error     | StdDev    | Gen0   | Allocated |
|------------------ |---------:|----------:|----------:|-------:|----------:|
| JawboneUdpV4      | 2.903 μs | 0.0101 μs | 0.0089 μs |      - |         - |
| JawboneUdpV6      | 2.605 μs | 0.0105 μs | 0.0098 μs |      - |         - |
| SystemUdpClientV4 | 3.385 μs | 0.0084 μs | 0.0078 μs | 0.0229 |     424 B |
| SystemUdpClientV6 | 3.085 μs | 0.0096 μs | 0.0085 μs | 0.0267 |     480 B |
| SystemUdpSocketV4 | 3.060 μs | 0.0533 μs | 0.0499 μs |      - |         - |
| SystemUdpSocketV6 | 2.834 μs | 0.0132 μs | 0.0123 μs |      - |         - |
