using Xunit;

namespace System.Tests;

public class ObjectExtensionsTests
{
    [Theory]
    [InlineData(null)]
    public void ThrowIfNullTest(object obj)
    {
        Assert.Equal("obj", Assert.Throws<ArgumentNullException>(() => obj.ThrowIfNull(nameof(obj))).ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ThrowIfBlankTest(string str)
    {
        if (str == null)
            Assert.Equal("str", Assert.Throws<ArgumentNullException>(() => str.ThrowIfBlank(nameof(str))).ParamName);
        else
            Assert.Equal("str", Assert.Throws<ArgumentException>(() => str.ThrowIfBlank(nameof(str))).ParamName);
    }
}
