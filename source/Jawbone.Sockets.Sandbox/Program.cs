// See https://aka.ms/new-console-template for more information
using Jawbone.Sockets;
using System;

IpAddressV6[] addresses =
[
    IpAddressV6.Local,
    (IpAddressV6)IpAddressV4.Local,
    (IpAddressV6)IpAddressV4.Broadcast,
    IpAddressV6.Local
];

addresses[^1].ScopeId = 127;
foreach (var a in addresses)
{
    var text = a.ToString();
    Console.WriteLine("string: " + text);
    Console.WriteLine("parsed: " + IpAddressV6.Parse(text));
}

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
