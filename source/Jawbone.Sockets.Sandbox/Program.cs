// See https://aka.ms/new-console-template for more information
using Jawbone.Sockets;
using System;

int index = 1;
foreach (var endpoint in Dns.Query("google.com"))
    Console.WriteLine($"{index++}: {endpoint}");
