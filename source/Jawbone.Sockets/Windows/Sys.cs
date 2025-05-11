using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Jawbone.Sockets.Windows;

static unsafe partial class Sys
{
    public const string Lib = "ws2_32";
    public static nuint InvalidSocket => nuint.MaxValue;
    private static WsaData s_wsaData;

    static Sys()
    {
        var result = WsaStartup(0x0202, out s_wsaData);
        if (result != 0)
        {
            throw new SocketException("Unable to initialize Windows networking.")
            {
                Code = Error.GetErrorCode(result)
            };
        }
    }

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-wsastartup
    [LibraryImport(Lib, EntryPoint = "WSAStartup")]
    public static partial int WsaStartup(ushort versionRequired, out WsaData wsaData);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-wsagetlasterror
    [LibraryImport(Lib, EntryPoint = "WSAGetLastError")]
    public static partial int WsaGetLastError();

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-socket
    [LibraryImport(Lib, EntryPoint = "socket")]
    public static partial nuint Socket(int af, int type, int protocol);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-bind
    [LibraryImport(Lib, EntryPoint = "bind")]
    public static partial int BindV4(nuint s, in SockAddrIn name, int namelen);

    [LibraryImport(Lib, EntryPoint = "bind")]
    public static partial int BindV6(nuint s, in SockAddrIn6 name, int namelen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-send
    [LibraryImport(Lib, EntryPoint = "send")]
    public static partial int Send(nuint s, in byte buf, int len, int flags);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-sendto
    [LibraryImport(Lib, EntryPoint = "sendto")]
    public static partial nint SendToV4(
        nuint s,
        in byte buf,
        nuint len,
        int flags,
        in SockAddrIn to,
        int tolen);

    [LibraryImport(Lib, EntryPoint = "sendto")]
    public static partial nint SendToV6(
        nuint s,
        in byte buf,
        nuint len,
        int flags,
        in SockAddrIn6 to,
        int tolen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-recv
    [LibraryImport(Lib, EntryPoint = "recv")]
    public static partial int Recv(nuint s, out byte buf, int len, int flags);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-recvfrom
    [LibraryImport(Lib, EntryPoint = "recvfrom")]
    public static partial nint RecvFrom(
        nuint s,
        out byte buf,
        int len,
        int flags,
        out SockAddrStorage from,
        ref int fromlen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-getsockname
    [LibraryImport(Lib, EntryPoint = "getsockname")]
    public static partial int GetSockName(nuint s, out SockAddrStorage name, ref int namelen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-connect
    [LibraryImport(Lib, EntryPoint = "connect")]
    public static partial int ConnectV4(nuint s, in SockAddrIn name, int namelen);

    [LibraryImport(Lib, EntryPoint = "connect")]
    public static partial int ConnectV6(nuint s, in SockAddrIn6 name, int namelen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-listen
    [LibraryImport(Lib, EntryPoint = "listen")]
    public static partial int Listen(nuint s, int backlog);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-accept
    [LibraryImport(Lib, EntryPoint = "accept")]
    public static partial nuint Accept(nuint s, out SockAddrStorage addr, ref int namelen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-wsapoll
    [LibraryImport(Lib, EntryPoint = "WSAPoll")]
    public static partial int WsaPoll(ref WsaPollFd fds, uint nfds, int timeout);

    // https://learn.microsoft.com/en-us/windows/win32/api/ws2tcpip/nf-ws2tcpip-getaddrinfo
    [LibraryImport(Lib, EntryPoint = "getaddrinfo", StringMarshalling = StringMarshalling.Utf8)]
    public static partial int GetAddrInfo(string? node, string? service, in AddrInfo hints, out AddrInfo* res);

    // https://learn.microsoft.com/en-us/windows/win32/api/ws2tcpip/nf-ws2tcpip-freeaddrinfo
    [LibraryImport(Lib, EntryPoint = "freeaddrinfo")]
    public static partial void FreeAddrInfo(AddrInfo* res);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-setsockopt
    [LibraryImport(Lib, EntryPoint = "setsockopt")]
    public static partial int SetSockOpt(
        nuint s,
        int level,
        int optname,
        in uint optval,
        int optlen);

    // https://learn.microsoft.com/en-us/windows/win32/api/winsock2/nf-winsock2-closesocket
    [LibraryImport(Lib, EntryPoint = "closesocket")]
    public static partial int CloseSocket(nuint s);

    [DoesNotReturn]
    public static void Throw(string message)
    {
        var error = WsaGetLastError();
        Throw(error, message);
    }

    [DoesNotReturn]
    public static void Throw(int error, string message)
    {
        var errorCode = Error.GetErrorCode(error);
        var exception = new SocketException(message + " " + errorCode)
        {
            Code = errorCode
        };

        throw exception;
    }

    [InlineArray(512)] // Actual size is less than this. Just being safe.
    public struct WsaData
    {
        private byte _element0;
    }
}
