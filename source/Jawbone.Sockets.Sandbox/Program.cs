// See https://aka.ms/new-console-template for more information
using Jawbone.Sockets;
using System;

{
    var v4 = IpNetwork<IpAddressV4>.Parse("127.0.0.1/32");
    DumpRange(v4);
    var v6 = IpNetwork<IpAddressV6>.Parse("ffff::/120");
    DumpRange(v6);
}

{
    var v4 = IpNetwork<IpAddressV4>.Parse("198.51.100.0/22");
    DumpRange(v4);
    var v6 = IpNetwork<IpAddressV6>.Parse("2001:db8::/48");
    DumpRange(v6);
}

{
    IpNetwork v4 = IpAddressV4.LinkLocalNetwork;
    Console.WriteLine(v4);
    IpNetwork v6 = IpAddressV6.LinkLocalNetwork;
    Console.WriteLine(v6);
    Environment.Exit(0);
}

{
    var e4 = IpAddressV4.Local.OnPort(5555);
    var e6 = IpAddressV6.Local.OnPort(5555);
    Console.WriteLine(e4);
    Console.WriteLine(e6);
}

{
    var address = new IpAddressV4(198, 51, 100, 0);
    var network = IpNetwork.Create(address, 24);
    // Console.WriteLine(network);
    Test(network, new IpAddressV4(198, 51, 100, 14));
    Test(network, new IpAddressV4(198, 51, 101, 14));
}

{
    var address = IpAddressV6.Parse("2001:db8::");
    var network = IpNetwork.Create(address, 48);
    Test(network, IpAddressV6.Parse("2001:db8:0:ffff:ffff:ffff:ffff:ffff"));
    Test(network, IpAddressV6.Parse("2001:db8:1::"));
}

{
    var address = new IpAddressV6();
    address.DataU32[..].Fill(uint.MaxValue);
    address.ScopeId = uint.MaxValue;
    var network = IpNetwork.Create(address, 128);
    var text = network.ToString();
    Console.WriteLine(text);
    Console.WriteLine(text.Length);
}

{
    var input = "fe80::67c:1:fe0:b237%enp14s0";
    var address = Dns.GetAddressV6(input);
    Console.WriteLine(input);
    Console.WriteLine(address.ToString());

    Console.WriteLine(IpAddressV4.Broadcast.OnPort(1000).ToString());
    Console.WriteLine(IpAddressV6.Local.OnPort(1000));
}

// IpAddressV6[] addresses =
// [
//     IpAddressV6.Local,
//     (IpAddressV6)IpAddressV4.Local,
//     (IpAddressV6)IpAddressV4.Broadcast,
//     IpAddressV6.Local
// ];

// addresses[^1].ScopeId = 127;
// foreach (var a in addresses)
// {
//     var text = a.ToString();
//     Console.WriteLine("string: " + text);
//     Console.WriteLine("parsed: " + IpAddressV6.Parse(text));
// }

// var values = new byte[16];
// values.AsSpan().Fill(byte.MaxValue);
// var address = new IpAddressV6(values, uint.MaxValue);
// var endpoint = address.OnPort(ushort.MaxValue);
// var text = endpoint.ToString();
// Console.WriteLine(text);
// Console.WriteLine(text.Length);

// int index = 1;
// foreach (var endpoint in Dns.Query("google.com"))
//     Console.WriteLine($"{index++}: {endpoint}");

static void Test<T>(IpNetwork<T> network, T address)
    where T : unmanaged, IIpAddress<T>
{
    var result = address.IsInNetwork(network);
    Console.WriteLine($"Network {network} - Test {address} - {result}");
}

static void DumpRange<T>(IpNetwork<T> ipNetwork) where T : unmanaged, IIpAddress<T>
{
    Console.WriteLine(
        $"{ipNetwork} to {T.GetMaxAddress(ipNetwork)}");
}
