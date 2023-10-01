// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson.Extensions;

public class DbFileServiceTests
{
    private readonly IDbFileService _dbFile;

    public DbFileServiceTests()
    {
        _dbFile = new DbFileService(Configuration);
    }

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<byte>() },
            new object[] { TestBytes1k },
            new object[] { TestBytes5k },
            new object[] { TestBytes55k }
        };

        return allData.Take(numTests);
    }

    public static IEnumerable<object[]> GetTestString(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { TestString1k },
            new object[] { TestString5k },
            new object[] { TestString55k }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
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
