namespace Jawbone.Sockets;

static class ExceptionMessages
{
    public const string OpenSocket = "Unable to open socket.";
    public const string CloseSocket = "Unable to close socket.";
    public const string SendDatagram = "Unable to send datagram.";
    public const string SendStream = "Unable to send data.";
    public const string ReceiveData = "Unable to receive data.";
    public const string Poll = "Unable to poll socket.";
    public const string GetSocketName = "Unable to get socket name.";
    public const string Accept = "Unable to accept socket.";
    public const string TcpNoDelay = "Unable to enable TCP_NODELAY.";
}
