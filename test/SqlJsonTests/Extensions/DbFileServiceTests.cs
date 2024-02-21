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

    public static readonly TheoryData<byte[]> GetTestBytes = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };
    
    public static readonly TheoryData<string> GetTestStrings = new() { string.Empty, TestString1k, TestString5k, TestString55k };

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public async Task BinFileTests(byte[] bytes)
    {
        var fileName = $"{DateTime.Now:HHmmss}.bin";
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
    [MemberData(nameof(GetTestStrings))]
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
