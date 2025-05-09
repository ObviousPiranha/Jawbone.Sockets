using System;

namespace Piranha.Sockets;

static class CreateExceptionFor
{
    public static InvalidOperationException InvalidAddressSize(long addrLen) => new("Unsupported address size: " + addrLen);
    public static InvalidOperationException BadPoll() => new("Unexpected poll event.");
}
