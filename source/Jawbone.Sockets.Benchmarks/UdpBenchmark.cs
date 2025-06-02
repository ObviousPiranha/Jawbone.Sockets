using BenchmarkDotNet.Attributes;
using System;
using System.IO;
using System.Net;

namespace Jawbone.Sockets.Benchmarks;

[MemoryDiagnoser]
public class UdpBenchmark : IDisposable
{
    private readonly System.Net.Sockets.UdpClient _oldUdpClientV4A;
    private readonly System.Net.Sockets.UdpClient _oldUdpClientV4B;
    private readonly System.Net.Sockets.UdpClient _oldUdpClientV6A;
    private readonly System.Net.Sockets.UdpClient _oldUdpClientV6B;

    private readonly System.Net.Sockets.Socket _oldSocketV4A;
    private readonly System.Net.Sockets.Socket _oldSocketV4B;
    private readonly System.Net.Sockets.Socket _oldSocketV6A;
    private readonly System.Net.Sockets.Socket _oldSocketV6B;

    private readonly IUdpSocket<IpAddressV4> _newUdpSocketV4;
    private readonly IUdpClient<IpAddressV4> _newUdpClientV4;
    private readonly IUdpSocket<IpAddressV6> _newUdpSocketV6;
    private readonly IUdpClient<IpAddressV6> _newUdpClientV6;

    private readonly byte[] _outBuffer = new byte[256];
    private readonly byte[] _inBuffer = new byte[512];


    public UdpBenchmark()
    {
        Random.Shared.NextBytes(_outBuffer);
        var portV4 = Random.Shared.Next(20000, 60000);
        _oldUdpClientV4A = new(portV4);
        _oldUdpClientV4B = new();
        _oldUdpClientV4B.Connect(IPAddress.Loopback, portV4);

        var portV6 = Random.Shared.Next(20000, 60000);
        _oldUdpClientV6A = new(portV6, System.Net.Sockets.AddressFamily.InterNetworkV6);
        _oldUdpClientV6B = new(System.Net.Sockets.AddressFamily.InterNetworkV6);
        _oldUdpClientV6B.Connect(IPAddress.IPv6Loopback, portV6);

        _oldSocketV4A = new(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketV4A.Bind(new IPEndPoint(IPAddress.Loopback, portV4 + 1));
        _oldSocketV4B = new(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketV4B.Connect(new IPEndPoint(IPAddress.Loopback, portV4 + 1));

        _oldSocketV6A = new(
            System.Net.Sockets.AddressFamily.InterNetworkV6,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketV6A.Bind(new IPEndPoint(IPAddress.IPv6Loopback, portV6 + 1));
        _oldSocketV6B = new(
            System.Net.Sockets.AddressFamily.InterNetworkV6,
            System.Net.Sockets.SocketType.Dgram,
            System.Net.Sockets.ProtocolType.Udp);
        _oldSocketV6B.Connect(new IPEndPoint(IPAddress.IPv6Loopback, portV6 + 1));

        _newUdpSocketV4 = UdpSocketV4.BindLocalIp();
        var endpointV4 = _newUdpSocketV4.GetSocketName();
        _newUdpClientV4 = UdpClientV4.Connect(IpAddressV4.Local.OnPort(endpointV4.Port));

        _newUdpSocketV6 = UdpSocketV6.BindLocalIp();
        var endpointV6 = _newUdpSocketV6.GetSocketName();
        _newUdpClientV6 = UdpClientV6.Connect(IpAddressV6.Local.OnPort(endpointV6.Port));
    }

    public void Dispose()
    {
        _newUdpClientV4.Dispose();
        _newUdpSocketV4.Dispose();
        _newUdpClientV6.Dispose();
        _newUdpSocketV6.Dispose();
        _oldSocketV4B.Dispose();
        _oldSocketV4A.Dispose();
        _oldSocketV6B.Dispose();
        _oldSocketV6A.Dispose();
        _oldUdpClientV4B.Dispose();
        _oldUdpClientV4A.Dispose();
        _oldUdpClientV6B.Dispose();
        _oldUdpClientV6A.Dispose();
    }

    [Benchmark]
    public void JawboneUdpV4()
    {
        _newUdpClientV4.Send(_outBuffer);
        var result = _newUdpSocketV4.Receive(_inBuffer, 1000, out var origin);
        if (result.Count != _outBuffer.Length)
            throw new InvalidDataException();
    }

    [Benchmark]
    public void JawboneUdpV6()
    {
        _newUdpClientV6.Send(_outBuffer);
        var result = _newUdpSocketV6.Receive(_inBuffer, 1000, out var origin);
        if (result.Count != _outBuffer.Length)
            throw new InvalidDataException();
    }

    [Benchmark]
    public void SystemUdpClientV4()
    {
        _oldUdpClientV4B.Send(_outBuffer);
        if (_oldUdpClientV4A.Client.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var endpoint = default(IPEndPoint);
            var result = _oldUdpClientV4A.Receive(ref endpoint);
            if (result.Length != _outBuffer.Length)
                throw new InvalidDataException();
        }
        else
        {
            throw new InvalidDataException();
        }
    }

    [Benchmark]
    public void SystemUdpClientV6()
    {
        _oldUdpClientV6B.Send(_outBuffer);
        if (_oldUdpClientV6A.Client.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var endpoint = default(IPEndPoint);
            var result = _oldUdpClientV6A.Receive(ref endpoint);
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
        _oldSocketV4B.Send(_outBuffer);
        if (_oldSocketV4A.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var result = _oldSocketV4A.Receive(_inBuffer);
            if (result != _outBuffer.Length)
                throw new InvalidDataException();
        }
        else
        {
            throw new InvalidDataException();
        }
    }

    [Benchmark]
    public void SystemUdpSocketV6()
    {
        _oldSocketV6B.Send(_outBuffer);
        if (_oldSocketV6A.Poll(1000000, System.Net.Sockets.SelectMode.SelectRead))
        {
            var result = _oldSocketV6A.Receive(_inBuffer);
            if (result != _outBuffer.Length)
                throw new InvalidDataException();
        }
        else
        {
            throw new InvalidDataException();
        }
    }
}
