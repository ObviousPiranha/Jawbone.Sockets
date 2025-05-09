namespace Piranha.Sockets.Windows;

static class Poll
{
    public const short RdNorm = 1 << 8;
    public const short RdBand = 1 << 9;
    public const short In = RdNorm | RdBand;
    public const short Pri = 1 << 10;
    public const short WrNorm = 1 << 4;
    public const short Out = WrNorm;
    public const short WrBand = 1 << 5;
    public const short Err = 1 << 0;
    public const short Hup = 1 << 1;
    public const short Nval = 1 << 2;
}
