using SqlJsonTests;
using System.Text;
using UkrGuru.SqlJson;
using Xunit;

namespace UkrGuru.Extensions;

public class WJbFileTests
{
    public WJbFileTests() => DbHelper.ConnectionString = Globals.ConnectionString;

    [Theory]
    [InlineData("file.html", "<html><head></head><title>TEST</title><body></body></html>")]
    public async Task CanCompressDecompress(string filename, string content, CancellationToken cancellationToken = default)
    {
        var sourceBytes = Encoding.UTF8.GetBytes(content);

        WJbFile file = new() { FileName = filename, FileContent = sourceBytes };

        await file.CompressAsync(cancellationToken);

        Assert.EndsWith(".gzip", file.FileName);

        await file.DecompressAsync(cancellationToken);

        Assert.Equal(filename, file.FileName);

        Assert.Equal(sourceBytes, file.FileContent);
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    public async Task WJbFileHelperTests(string content, CancellationToken cancellationToken = default)
    {
        var guid1 = await WJbFileHelper.SetAsync(content, cancellationToken: cancellationToken);

        var guid2 = await WJbFileHelper.SetAsync(content, cancellationToken: cancellationToken);

        Assert.Equal(guid1, guid2);

        var content1 = await WJbFileHelper.GetAsync(guid1, cancellationToken);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await WJbFileHelper.DelAsync(guid, cancellationToken);

            content1 = await WJbFileHelper.GetAsync(guid1, cancellationToken);

            Assert.Null(content1);
        }
    }
}