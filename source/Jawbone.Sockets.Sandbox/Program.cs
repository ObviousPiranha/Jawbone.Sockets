// See https://aka.ms/new-console-template for more information
using Jawbone.Sockets;
using System;

var values = new byte[16];
values.AsSpan().Fill(byte.MaxValue);
var address = new IpAddressV6(values, uint.MaxValue);
var endpoint = address.OnPort(ushort.MaxValue);
var text = endpoint.ToString();
Console.WriteLine(text);
Console.WriteLine(text.Length);

// int index = 1;
// foreach (var endpoint in Dns.Query("google.com"))
//     Console.WriteLine($"{index++}: {endpoint}");
