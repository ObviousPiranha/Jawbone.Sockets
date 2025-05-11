using System;

namespace Jawbone.Sockets;

public struct TransferResult
{
    public SocketResult Result;
    public int Count;

    public TransferResult(SocketResult result) => Result = result;
    public TransferResult(int count)
    {
        Result = SocketResult.Success;
        Count = count;
    }
}
