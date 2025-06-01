using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jawbone.Sockets.Samples;

file static class Extensions
{
    public static T GetOrDefault<T>(this ReadOnlySpan<T> span, int index, T defaultValue)
    {
        return 0 <= index && index < span.Length ? span[index] : defaultValue;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        try
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please specify sample application name:");
                Console.WriteLine("  - chat");
            }
            else
            {
                switch (args[0])
                {
                    case "chat":
                        ChatApp(args.AsSpan(1));
                        break;
                    default:
                        Console.WriteLine("Unrecognized app: " + args[0]);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex);
        }

        Console.WriteLine();
    }

    static void ChatApp(ReadOnlySpan<string> args)
    {
        if (1 < args.Length)
            ChatAppClient(args[0], args[1]);
        else if (0 < args.Length)
            ChatAppServer(args[0]);
        else
            Console.WriteLine("Specify port to run server. Specify port and host to run client.");
    }

    static void ChatAppClient(string port, string host)
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var info = AddressInfo.Get(host, port);
        var endpoint = info.V4[0];

        var client = UdpClientV4.Connect(endpoint);

        var thread = new Thread(() =>
        {
            try
            {
                var inBuffer = new byte[2048];
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var result = client.Receive(inBuffer, 1000);
                    if (0 < result.Count)
                    {
                        var message = inBuffer.AsSpan(0, result.Count);
                        var text = Encoding.UTF8.GetString(message);
                        Console.Write(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex);
                Console.WriteLine();
            }
        });

        thread.Start();

        try
        {
            var outBuffer = new byte[2048];

            while (true)
            {
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    break;

                var writer = SpanWriter.Create(outBuffer);
                writer.WriteAsUtf8(input);

                client.Send(writer.Written);
            }

            client.Send("$DISCONNECT"u8);
            Console.WriteLine("Exiting...");
            cancellationTokenSource.Cancel();
        }
        finally
        {
            thread.Join();
        }
    }

    static void ChatAppServer(string port)
    {
        using var semaphore = new SemaphoreSlim(0);
        var clients = new Dictionary<IpEndpoint<IpAddressV4>, long>();
        var deadClients = new List<IpEndpoint<IpAddressV4>>();
        using var server = UdpSocketV4.BindAnyIp(int.Parse(port));
        server.HandleInterruptOnReceive = InterruptHandling.Abort;
        Console.WriteLine($"Server listening on port {port}");

        const int ClientTimeout = 8000;
        var inBuffer = new byte[2048];
        var outBuffer = new byte[2048];
        var running = true;
        var stopped = false;

        Console.CancelKeyPress += (_, e) =>
        {
            if (!stopped)
            {
                Console.WriteLine("Stopping server...");
                e.Cancel = true;
                stopped = true;
                semaphore.Release();
            }
        };

        while (running)
        {
            var writer = SpanWriter.Create(outBuffer);
            var timeout = ClientTimeout;
            var now = Environment.TickCount64;
            deadClients.Clear();
            foreach (var pair in clients)
            {
                var age = (int)(now - pair.Value);
                if (ClientTimeout < age)
                    deadClients.Add(pair.Key);
                else
                    timeout = int.Min(timeout, ClientTimeout - age);
            }

            foreach (var client in deadClients)
            {
                Console.WriteLine($"{client} timed out.");
                clients.Remove(client);
                server.Send("You have been disconnected.\n"u8, client);

                writer.WriteAsUtf8(client.ToString());
                writer.Write(" timed out.\n"u8);
            }

            if (!writer.Written.IsEmpty)
            {
                foreach (var endpoint in clients.Keys)
                    server.Send(writer.Written, endpoint);
            }

            writer.Reset();

            var result = server.Receive(inBuffer, timeout, out var origin);
            if (0 < result.Count)
            {
                var message = inBuffer.AsSpan(0, result.Count);
                var messageUtf16 = Encoding.UTF8.GetString(message);
                Console.WriteLine($"{origin} sent {messageUtf16}");
                if (message.SequenceEqual("$DISCONNECT"u8))
                {
                    if (clients.Remove(origin))
                    {
                        writer.WriteAsUtf8(origin.ToString());
                        writer.Write(" disconnected.\n"u8);

                        foreach (var endpoint in clients.Keys)
                            server.Send(writer.Written, endpoint);
                    }
                }
                else
                {
                    if (!clients.ContainsKey(origin))
                    {
                        writer.WriteAsUtf8(origin.ToString());
                        writer.Write(" connected!\n"u8);
                    }

                    clients[origin] = Environment.TickCount64;

                    writer.WriteAsUtf8(origin.ToString());
                    writer.Write(" -- "u8);
                    writer.Write(message);
                    writer.Write((byte)'\n');

                    foreach (var endpoint in clients.Keys)
                    {
                        if (endpoint == origin)
                            continue;

                        server.Send(writer.Written, endpoint);
                    }
                }
            }
            else if (result.Result == SocketResult.Interrupt)
            {
                running = false;
                semaphore.Wait();
            }
        }
    }
}
