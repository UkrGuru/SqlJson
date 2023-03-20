// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions.Data;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

public class DbFileHelperTests
{
    public DbFileHelperTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

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
    public async Task CanZipFile(byte[] bytes)
    {
        var fileName = $"{DateTime.Now.ToString("HHmmss")}.bin";

        DbFile file = new() { FileName = fileName, FileContent = bytes };

        await file.CompressAsync();

        Assert.EndsWith(".gzip", file.FileName);

        await file.DecompressAsync();

        Assert.Equal(fileName, file.FileName);

        Assert.Equal(bytes, file.FileContent);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 3)]
    public async Task BinFileTests(byte[] bytes)
    {
        var fileName = $"{DateTime.Now.ToString("HHmmss")}.bin";
        var file1 = new DbFile { FileName = fileName, FileContent = bytes };

        var guid = await file1.SetAsync<Guid?>();

        var file = await DbFileHelper.GetAsync(guid);

        Assert.Equal(fileName, file?.FileName);

        Assert.Equal(bytes, file?.FileContent);

        await DbHelper.ExecAsync("WJbFiles_Del", guid);
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
        var guid1 = await DbFileHelper.SetAsync(content);

        var content1 = await DbFileHelper.GetAsync(guid1);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await DbFileHelper.DelAsync(guid);

            content1 = await DbFileHelper.GetAsync(guid1);

            Assert.Null(content1);
        }
    }
}