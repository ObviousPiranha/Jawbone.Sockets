using BenchmarkDotNet.Attributes;
using System;
using System.IO;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class UdpBenchmark : IDisposable
{
    private readonly System.Net.Sockets.UdpClient _oldUdpClientA;
    private readonly System.Net.Sockets.UdpClient _oldUdpClientB;

    private readonly System.Net.Sockets.Socket _oldSocketA;
    private readonly System.Net.Sockets.Socket _oldSocketB;

    private readonly IUdpSocket<IpAddressV4> _newUdpSocket;
    private readonly IUdpClient<IpAddressV4> _newUdpClient;

    private readonly byte[] _outBuffer = new byte[256];
    private readonly byte[] _inBuffer = new byte[512];


    public UdpBenchmark()
    {
        Random.Shared.NextBytes(_outBuffer);
        var port = Random.Shared.Next(20000, 60000);
        _oldUdpClientA = new(port);
        _oldUdpClientB = new();
        _oldUdpClientB.Connect(IPAddress.Loopback, port);

        _oldSocketA = new(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketA.Bind(new IPEndPoint(IPAddress.Loopback, port + 1));
        _oldSocketB = new(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketB.Connect(new IPEndPoint(IPAddress.Loopback, port + 1));

        _newUdpSocket = UdpSocketV4.BindLocalIp();
        var endpoint = _newUdpSocket.GetSocketName();
        _newUdpClient = UdpClientV4.Connect(IpAddressV4.Local.OnPort(endpoint.Port));
    }

    public void Dispose()
    {
        _newUdpClient.Dispose();
        _newUdpSocket.Dispose();
        _oldSocketB.Dispose();
        _oldSocketA.Dispose();
        _oldUdpClientB.Dispose();
        _oldUdpClientA.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void JawboneUdpV4()
    {
        _newUdpClient.Send(_outBuffer);
        var result = _newUdpSocket.Receive(_inBuffer, 1000, out var origin);
        if (result.Count != _outBuffer.Length)
            throw new InvalidDataException();
    }

    [Benchmark]
    public void SystemUdpClientV4()
    {
        _oldUdpClientB.Send(_outBuffer);
        if (_oldUdpClientA.Client.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var endpoint = default(IPEndPoint);
            var result = _oldUdpClientA.Receive(ref endpoint);
            if (result.Length != _outBuffer.Length)
                throw new InvalidDataException();
        }
        else
        {
            throw new InvalidDataException();
        }
    }

    [Benchmark]
    public void SystemUdpSocketV4()
    {
        _oldSocketB.Send(_outBuffer);
        if (_oldSocketA.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var result = _oldSocketA.Receive(_inBuffer);
            if (result != _outBuffer.Length)
                throw new InvalidDataException();
        }
        else
        {
            throw new InvalidDataException();
        }
    }
}
