// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.Client.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions;

public class ApiDbFileServiceTests
{
    private readonly IDbFileService _dbFile;
    
    public ApiDbFileServiceTests()
    {
        _dbFile = new ApiDbFileService(Http);
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

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task BinFileTests(byte[] bytes)
    {
        var fileName = $"{DateTime.Now.ToString("HHmmssnnn")}.bin";
        var file1 = new DbFile { FileName = fileName, FileContent = bytes };
        var guid1 = await _dbFile.SetAsync(file1);

        var file3 = await _dbFile.GetAsync(guid1);
        Assert.Equal(fileName, file3!.FileName);
        Assert.Equal(bytes, file3!.FileContent!);

        await _dbFile.DelAsync(guid1);
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
    [MemberData(nameof(GetTestString), parameters: 4)]
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