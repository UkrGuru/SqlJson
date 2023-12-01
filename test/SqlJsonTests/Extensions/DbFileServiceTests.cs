// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions.Tests;

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
        var file = new DbFile { FileName = fileName, FileContent = bytes };

        var guid = await file.SetAsync<Guid?>();

        if (bytes?.Length > 0)
        {
            var fileActual = await _dbFile.GetAsync(guid);

            Assert.Equal(fileName, fileActual?.FileName);

            Assert.Equal(bytes, fileActual?.FileContent);

            await _dbFile.DelAsync(guid);
        }
        else
        {
            Assert.Null(guid);
        }
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task TxtFileTests(string content)
    {
        var guid = await _dbFile.SetAsync(content);

        if (content?.Length > 0)
        {
            var contentActual = await _dbFile.GetAsync(guid);

            Assert.Equal(content, contentActual);

            if (!string.IsNullOrEmpty(guid) && Guid.TryParse(guid, out Guid guidNew))
            {
                await _dbFile.DelAsync(guidNew);

                contentActual = await _dbFile.GetAsync(guid);

                Assert.Null(contentActual);
            }
        }
        else
        {
            Assert.Equal(content, guid);
        }
    }
}
