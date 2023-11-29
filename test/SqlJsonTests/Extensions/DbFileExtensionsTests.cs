using System.Text;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbFileExtensionsTests
{
    [Fact]
    public async Task ToStringAsync_Null()
    {
        DbFile? file = null;

        var result = await file.ToStringAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task ToStringAsync_Empty()
    {
        var file = new DbFile { FileContent = Encoding.UTF8.GetBytes(string.Empty) };

        var result = await file.ToStringAsync();

        Assert.Empty(result!);
    }

    [Fact]
    public async Task ToStringAsync_NotEmpty()
    {
        var file = new DbFile { FileContent = Encoding.UTF8.GetBytes("Hello, world!") };

        var result = await file.ToStringAsync();

        Assert.Equal("Hello, world!", result);
    }
}