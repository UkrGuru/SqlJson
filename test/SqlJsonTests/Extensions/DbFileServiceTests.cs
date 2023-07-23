// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Extensions;

public class DbFileServiceTests
{
    private readonly IConfiguration _configuration;

    private readonly IDbFileService _dbFile;

    public DbFileServiceTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        var inMemorySettings = new Dictionary<string, string?>() {
            { "ConnectionStrings:DefaultConnection", GlobalTests.ConnectionString},
            { "Logging:LogLevel:UkrGuru.SqlJson", "Information" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _dbFile = new DbFileService(_configuration);
    }

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { GlobalTests.TestBytes1k },
            new object[] { GlobalTests.TestBytes5k },
            new object[] { GlobalTests.TestBytes55k }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 3)]
    public async Task BinFileTests(byte[] bytes)
    {
        var fileName = $"{DateTime.Now.ToString("HHmmss")}.bin";
        var file1 = new DbFile { FileName = fileName, FileContent = bytes };

        var guid = await _dbFile.SetAsync(file1);

        var file = await _dbFile.GetAsync(guid);

        Assert.Equal(fileName, file?.FileName);

        Assert.Equal(bytes, file?.FileContent);

        await _dbFile.DelAsync(guid);
    }

    public static IEnumerable<object[]> GetTestString(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { GlobalTests.TestString1k },
            new object[] { GlobalTests.TestString5k },
            new object[] { GlobalTests.TestString55k }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 3)]
    public async Task TxtFileTests(string content)
    {
        var guid1 = await _dbFile.SetAsync(content);

        var content1 = await _dbFile.GetAsync(guid1);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await _dbFile.DelAsync(guid);

            content1 = await _dbFile.GetAsync(guid1);

            Assert.Null(content1);
        }
    }
}
