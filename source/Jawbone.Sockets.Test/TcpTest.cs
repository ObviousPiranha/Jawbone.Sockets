using System;

namespace Jawbone.Sockets.Test;

public class TcpTest
{
    private const int Timeout = 1000;

    [Fact]
    public void SendAndReceiveTcpV4()
    {
        using var listener = TcpListener.ListenAnyIpV4(1);
        var endpoint = listener.GetSocketName();
        using var client = TcpClient.Connect(IpAddressV4.Local.OnPort(endpoint.Port));
        using var server = listener.Accept(Timeout);
        Assert.NotNull(server);

        var message = "greetings"u8;
        var sendResult = client.Send(message);
        Assert.Equal(message.Length, sendResult.Count);

        Span<byte> buffer = new byte[64];
        var receiveResult = server.Receive(buffer, Timeout);
        Assert.Equal(SocketResult.Success, receiveResult.Result);
        Assert.Equal(message.Length, receiveResult.Count);
        Assert.Equal(message, buffer[..receiveResult.Count]);
        Assert.False(client.HungUp);
    }

    [Fact]
    public void SendAndReceiveTcpV6()
    {
        using var listener = TcpListener.ListenAnyIpV6(1);
        var endpoint = listener.GetSocketName();
        using var client = TcpClient.Connect(IpAddressV6.Local.OnPort(endpoint.Port));
        using var server = listener.Accept(Timeout);
        Assert.NotNull(server);

        var message = "greetings"u8;
        var sendResult = client.Send(message);
        Assert.Equal(message.Length, sendResult.Count);

        Span<byte> buffer = new byte[64];
        var receiveResult = server.Receive(buffer, Timeout);
        Assert.Equal(SocketResult.Success, receiveResult.Result);
        Assert.Equal(message.Length, receiveResult.Count);
        Assert.Equal(message, buffer[..receiveResult.Count]);
    }

    [Fact]
    public void CanSendTcpV4ToV6WhenAllowed()
    {
        var bindEndpoint = ((IpAddressV6)IpAddressV4.Local).OnAnyPort();
        using var listener = TcpListener.Listen(bindEndpoint, 1, SocketOptions.EnableDualMode);
        var listenerName = listener.GetSocketName();
        var endpointV4 = IpAddressV4.Local.OnPort(listenerName.Port);
        using var client = TcpClient.Connect(endpointV4);
        using var server = listener.Accept(Timeout);
        Assert.NotNull(server);

        var message = "greetings"u8;
        var sendResult = client.Send(message);
        Assert.Equal(message.Length, sendResult.Count);

        Span<byte> buffer = new byte[64];
        var receiveResult = server.Receive(buffer, Timeout);
        Assert.Equal(SocketResult.Success, receiveResult.Result);
        Assert.Equal(message.Length, receiveResult.Count);
        Assert.Equal(message, buffer[..receiveResult.Count]);
    }
}
