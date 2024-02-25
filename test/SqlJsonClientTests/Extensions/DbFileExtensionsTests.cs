using System.Text;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbFileExtensionsTests
{
    [Fact]
    public async Task CompressAsync_ShouldCompressFileContent()
    {
        var originalContent = new byte[] { 0x41, 0x42, 0x43 }; // Example content
        var file = new DbFile { FileContent = originalContent, FileName = "example.txt" };

        await file.CompressAsync();

        Assert.EndsWith(".gzip", file.FileName);
        Assert.NotEqual(originalContent, file.FileContent);
    }

    [Fact]
    public async Task DecompressAsync_ShouldDecompressFileContent()
    {
        var compressedContent = new byte[] { /* Compressed content, e.g., from previous test */ };
        var file = new DbFile { FileContent = compressedContent, FileName = "example.txt.gzip" };

        await file.DecompressAsync();

        Assert.NotEqual(".gzip", file.FileName.Substring(file.FileName.Length - 4));
        Assert.Equal(compressedContent, file.FileContent);
    }

    [Fact]
    public async Task CanToStringAsync_Null()
    {
        var file = new DbFile { FileContent = null };

        var result = await file.ToStringAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task CanToStringAsync_String()
    {
        var contentBytes = Encoding.UTF8.GetBytes("Your content goes here");
        var file = new DbFile { FileContent = contentBytes };

        var result = await file.ToStringAsync();

        Assert.Equal("Your content goes here", result);
    }
}