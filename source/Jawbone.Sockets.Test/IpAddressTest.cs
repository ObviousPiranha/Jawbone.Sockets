namespace Jawbone.Sockets.Test;

public class IpAddressTest
{
    [Fact]
    public void TypeMismatchNotEqual()
    {
        IpAddress none = default;
        IpAddress v4 = new IpAddressV4();
        Assert.Equal(IpAddressVersion.V4, v4.Version);
        IpAddress v6 = new IpAddressV6();
        Assert.Equal(IpAddressVersion.V6, v6.Version);
        Assert.False(none == v4);
        Assert.True(none != v4);
        Assert.False(none == v6);
        Assert.True(none != v6);
        Assert.False(v4 == v6);
        Assert.True(v4 != v6);
    }
}
