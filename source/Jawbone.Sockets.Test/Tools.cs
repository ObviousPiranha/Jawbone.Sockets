using System.Runtime.CompilerServices;

namespace Jawbone.Sockets.Test;

sealed class Tools
{
    public static void AssertSize<T>(int expected) where T : unmanaged
    {
        Assert.Equal(expected, Unsafe.SizeOf<T>());
    }
}