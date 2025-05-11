namespace Jawbone.Sockets.Linux;

struct PollFd
{
    public int Fd;
    public short Events;
    public short REvents;
}
