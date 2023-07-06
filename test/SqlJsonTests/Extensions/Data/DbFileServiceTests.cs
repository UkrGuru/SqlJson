// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

public class DbFileServiceTests
{
    public DbFileServiceTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

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
        IDbFileService db = new DbFileService(GlobalTests.Configuration);

        var fileName = $"{DateTime.Now.ToString("HHmmss")}.bin";
        var file1 = new DbFile { FileName = fileName, FileContent = bytes };

        var guid = await db.SetAsync(file1);

        var file = await db.GetAsync(guid);

        Assert.Equal(fileName, file?.FileName);

        Assert.Equal(bytes, file?.FileContent);

        await db.DelAsync(guid);
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
        IDbFileService db = new DbFileService(GlobalTests.Configuration);

        var guid1 = await db.SetAsync(content);

        var content1 = await db.GetAsync(guid1);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await db.DelAsync(guid);

            content1 = await db.GetAsync(guid1);

            Assert.Null(content1);
        }
    }
}
