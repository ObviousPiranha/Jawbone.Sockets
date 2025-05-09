namespace Piranha.Sockets.Mac;

struct PollFd
{
    public int Fd;
    public short Events;
    public short REvents;
}
