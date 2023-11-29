namespace UkrGuru.SqlJson.Extensions.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void CanThrowIfBlank_Null()
    {
        string? argument = null;

        Assert.Throws<ArgumentNullException>(() => argument.ThrowIfBlank());
    }

    [Fact]
    public void CanThrowIfBlank_Empty()
    {
        string? argument = string.Empty;

        Assert.Throws<ArgumentException>(() => argument.ThrowIfBlank());
    }

    [Fact]
    public void CanThrowIfBlank_Whitespace()
    {
        string? argument = "   ";

        Assert.Throws<ArgumentException>(() => argument.ThrowIfBlank());
    }

    [Fact]
    public void CanThrowIfBlank_NotBlank()
    {
        string argument = "Hello, world!";

        var result = argument.ThrowIfBlank();

        Assert.Equal(argument, result);
    }
}
