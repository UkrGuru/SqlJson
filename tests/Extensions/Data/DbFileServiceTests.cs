// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

public class DbFileServiceTests
{
    public DbFileServiceTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

    [Theory]
    [InlineData("asdasd12345678901234567890123456789012345678901234567890")]
    [InlineData("asdasd123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    public async Task CanSaveLoadDelete(string content)
    {
        DbFileService db = new DbFileService(GlobalTests.Configuration);

        var guid1 = await db.SetAsync(content);

        var guid2 = await db.SetAsync(content);

        Assert.Equal(guid1, guid2);

        var content1 = await db.GetAsync(guid1);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await db.DelAsync(guid);

            content1 = await db.GetAsync(guid1);

            Assert.Null(content1);
        }

        await db.GetAsync(null);

        await db.DelAsync(null);
    }

    [Fact]
    public async Task CanSaveBinFile()
    {
        DbFileService db = new DbFileService(GlobalTests.Configuration);

        var filename = $"{DateTime.Now.ToString("HHmmss")}.bin";
        var content = new byte[256 * 256]; for (int i = 0; i < 256; i++) for (int j = 0; j < 256; j++) content[i * 256 + j] = (byte)j;

        var guid = await db.ExecAsync<string?>("WJbFiles_Ins", new DbFile { FileName = filename, FileContent = content });

        var file = await db.ExecAsync<DbFile>("WJbFiles_Get", guid);

        Assert.Equal(filename, file?.FileName);

        Assert.Equal(content, file?.FileContent);

        await db.ExecAsync<DbFile>("WJbFiles_Del", guid);
    }
}
