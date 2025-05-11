namespace Jawbone.Sockets.Linux;

static class Poll
{
    public const short In = 1 << 0;
    public const short Pri = 1 << 1;
    public const short Out = 1 << 2;
    public const short Err = 1 << 3;
    public const short Hup = 1 << 4;
    public const short Nval = 1 << 5;
}
