// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson.Client;

public class ApiDbFileServiceTests
{
    private readonly HttpClient _http;
    private readonly IDbFileService _dbFile;

    public ApiDbFileServiceTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        _http = new HttpClient() { BaseAddress = new Uri("https://localhost:7285/") };

        _dbFile = new ApiDbFileService(_http);
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
        var fileName = $"{DateTime.Now.ToString("HHmmssnn")}.bin";
        var file1 = new DbFile { FileName = fileName, FileContent = bytes };

        var guid1 = await _dbFile.SetAsync(file1);

        var file = await _dbFile.GetAsync(guid1);

        Assert.Equal(fileName, file?.FileName);

        Assert.Equal(bytes, file?.FileContent);

        await _dbFile.DelAsync(guid1);
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