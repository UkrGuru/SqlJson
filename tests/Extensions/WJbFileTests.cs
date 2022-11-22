using System.Reflection;
using System.Text;
using UkrGuru.SqlJson;
using Xunit;

namespace UkrGuru.Extensions.Tests;

public class WJbFileTests
{
    private readonly bool dbOK = false;

    public WJbFileTests()
    {
        var dbName = "ExtensionsTest";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

        DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

        DbHelper.ConnectionString = connectionString;

        if ( dbOK ) return;

        var assembly = Assembly.GetAssembly(typeof(UkrGuru.Extensions.More));
        ArgumentNullException.ThrowIfNull(assembly);

        dbOK = assembly.InitDb();
    }

    [Fact]
    public void InitDbTest()
    {
        Assert.True(true);
    }

    [Theory]
    [InlineData("file.html", "<html><head></head><title>TEST</title><body></body></html>")]
    public async Task CompressDecompressTest(string filename, string content, CancellationToken cancellationToken = default)
    {
        var sourceBytes = Encoding.UTF8.GetBytes(content);

        WJbFile file = new() { FileName = filename, FileContent = sourceBytes };

        await file.CompressAsync(cancellationToken);

        Assert.EndsWith(".gzip", file.FileName);

        await file.DecompressAsync(cancellationToken);

        Assert.Equal(filename, file.FileName);

        Assert.Equal(sourceBytes, file.FileContent);
    }
}
