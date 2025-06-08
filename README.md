# Jawbone.Sockets

[![NuGet Version](https://img.shields.io/nuget/v/Jawbone.Sockets)](https://www.nuget.org/packages/Jawbone.Sockets/) [![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ObviousPiranha/Jawbone.Sockets/dotnet-desktop.yml)](https://github.com/ObviousPiranha/Jawbone.Sockets/actions)


UDP and TCP socket library for game engines!

The [TLDR](rant.md) is that the .NET socket libraries allocate way too much and support too many address families. This library focuses on the essentials.

See some [benchmark results here](benchmarks.md).

## Design

### IP Addresses

There are two address types: `IpAddressV4` and `IpAddressV6`. Both are structs. They are very simple to use.

```csharp
var host = new IpAddressV4(10, 0, 0, 23);

// Lots of shortcuts.
var localhost = IpAddressV4.Local;

// IPv6 is a little bigger.
var v6 = new IpAddressV6(55, 23, 11, 1, 9, 5, 22, 1, 0, 0, 0, 3, 12, 94, 201, 7);

// IPv6 also accepts spans.
var v6FromSpan = new IpAddressV6([55, 23, 11, 1, 9, 5, 22, 1, 0, 0, 0, 3, 12, 94, 201, 7]);

// Or parse it. Lots of options.
var parsedV6 = IpAddressV6.Parse("7f13:22e9::4000:910d");
```

### IP Endpoints

When you're ready to pair an address with a port, just use `IpEndpoint<T>` (also a struct).

```csharp
var endpointV4 = new IpEndpoint<IpAddressV4>(IpAddressV4.Local, 5000);
var endpointV6 = new IpEndpoint<IpAddressV6>(IpAddressV6.Local, 5000);

// Lots more shortcuts.
IpEndpoint<IpAddressV4> origin = IpEndpoint.Create(IpAddressV4.Local, 5000);

// Or you can use some extensions.
var host = new IpAddressV4(10, 0, 0, 23);
IpEndpoint<IpAddressV4> endpoint = host.OnPort(5000);
```

### DNS Queries

If you need to discover a host address, you have several options.

```csharp
// Iterate through each response using the non-generic IpEndpoint.
foreach (IpEndpoint endpoint in Dns.Query("github.com"))
{
    if (endpoint.Address.Version == IpAddressVersion.V4)
    {
        // Handle IPv4.
        var ep4 = (IpEndpoint<IpAddressV4>)endpoint;
    }

    if (endpoint.Address.Version == IpAddressVersion.V6)
    {
        // Handle IPv6.
        var ep6 = (IpEndpoint<IpAddressV6>)endpoint;
    }
}

foreach (IpEndpoint<IpAddressV4> endpoint in Dns.QueryV4("github.com"))
{
    // Handle just the IPv4 entries.
}

// Or use a shortcut to just grab the first address (without a port).
IpAddressV4 address = Dns.GetAddressV4("github.com");

// Use a safer method to avoid exceptions.
if (Dns.TryGetAddressV4("github.com", out IpAddressV4 addr))
{
    // ...
}
```

### UDP Sockets

Now you're ready to make a socket! All socket types are generic as they are _constrained_ to IPv4 or IPv6.

```csharp
// Create a socket and listen on port 10215. Ideal for servers.
using IUdpSocket<IpAddressV4> server = UdpSocket.BindAnyIpV4(10215);

// Connect a client.
IpEndpoint<IpAddressV4> origin = new IpAddressV4(10, 0, 0, 23).OnPort(10215);
using IUdpClient<IpAddressV4> client = UdpClient.Connect(origin);

// Create an IPv6 server and (optionally) allow interop with IPv4!
using IUdpSocket<IpAddressV6> serverV6 = UdpSocket.BindAnyIpV6(38555, allowV4: true);

// Connect an IPv6 client.
IpEndpoint<IpAddressV6> originV6 = myIpAddressV6.OnPort(38555);
using IUdpClient<IpAddressV6> clientV6 = UdpClient.Connect(originV6);
```

Sending data is very simple. The `Send` method accepts any `ReadOnlySpan<byte>`.

```csharp
var destination = IpAddressV4.Local.OnPort(10215);

// IUdpSocket needs a destination address.
server.Send("Hello!"u8, destination);

// IUdpClient is locked to a single address.
client.Send("Greetings!"u8);
```

Receiving data is only marginally more complex. It lets you specify a timeout in milliseconds. (Simply pick zero if you want a non-blocking call.)

```csharp
var buffer = new byte[2048];
var timeout = 1000; // One second
var result = server.Receive(buffer, timeout, out var sender);
if (0 < result.Count)
{
    var message = buffer.AsSpan(0, result.Count);
    // Handle received bytes here!
    Console.WriteLine($"Received {result.Count} bytes from host {sender}.");
}
else
{
    // Probably a timeout.
    // The field result.Result will tell you if it was a timeout or an interrupt.
}
```

### TCP Sockets

Create a TCP listener to get started.

```csharp
var bindEndpoint = IpAddressV4.Local.OnPort(5555);
using ITcpListener<IpAddressV4> listener = TcpListener.Listen(bindEndpoint, 4); // Backlog of 4 pending connections.
```

Connect with a client.

```csharp
using ITcpClient<IpAddressV4> client = TcpClient.Connect(serverEndpoint);
```

Accept the connection into another `ITcpClient<T>` on the server side.

```csharp
var timeout = 1000; // One second
using ITcpClient<IpAddressV4> server = listener.Accept(timeout);
if (server is null)
{
    // Null object just means it timed out or was interrupted.
}
```

Communicate back and forth with TCP goodness. All TCP sockets enable `TCP_NODELAY` automatically.

```csharp
client.Send("HTTP shenanigans"u8);

var result = server.Receive(buffer, timeout);
if (0 < result.Count)
{
    // Conquer the world here.
}
```
