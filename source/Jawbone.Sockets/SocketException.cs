using System;

namespace Jawbone.Sockets;

[Serializable]
public class SocketException : Exception
{
    public ErrorCode Code { get; init; }

    public SocketException() { }
    public SocketException(string message) : base(message) { }
    public SocketException(string message, Exception inner) : base(message, inner) { }

    internal static ErrorCode GetErrorCode(int error)
    {
        if (OperatingSystem.IsWindows())
            return Windows.Error.GetErrorCode(error);
        if (OperatingSystem.IsMacOS())
            return Mac.Error.GetErrorCode(error);
        if (OperatingSystem.IsLinux())
            return Linux.Error.GetErrorCode(error);
        throw new PlatformNotSupportedException();
    }
}
