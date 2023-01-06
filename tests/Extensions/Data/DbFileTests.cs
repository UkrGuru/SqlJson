// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions.Data;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

public class DbFileTests
{
    public DbFileTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100 ) { Thread.Sleep(100); } }

    [Theory]
    [InlineData("file.html", "<html><head></head><title>TEST</title><body>TESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTESTTEST</body></html>")]
    public async Task CanCompressDecompress(string filename, string content)
    {
        var sourceBytes = Encoding.UTF8.GetBytes(content);

        DbFile file = new() { FileName = filename, FileContent = sourceBytes };

        await file.CompressAsync();

        Assert.EndsWith(".gzip", file.FileName);

        await file.DecompressAsync();

        Assert.Equal(filename, file.FileName);

        Assert.Equal(sourceBytes, file.FileContent);
    }

    [Theory]
    [InlineData("12345678901234567890123456789012345678901234567890")]
    [InlineData("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    public async Task CanSaveLoadDelete(string content)
    {
        var guid1 = await DbFileHelper.SetAsync(content);

        var guid2 = await DbFileHelper.SetAsync(content);

        Assert.Equal(guid1, guid2);

        var content1 = await DbFileHelper.GetAsync(guid1);

        Assert.Equal(content, content1);

        if (!string.IsNullOrEmpty(guid1) && Guid.TryParse(guid1, out Guid guid))
        {
            await DbFileHelper.DelAsync(guid);

            content1 = await DbFileHelper.GetAsync(guid1);

            Assert.Null(content1);
        }

        await DbFileHelper.GetAsync(null);

        await DbFileHelper.DelAsync(null);
    }

    [Fact]
    public async Task CanSaveBinFile()
    {
        var filename = $"{DateTime.Now.ToString("HHmmss")}.bin";
        var content = new byte[256*256]; for (int i = 0; i < 256; i++) for (int j = 0; j < 256; j++) content[i*256 + j] = (byte)j;

        var guid = await DbHelper.ExecAsync<string?>("WJbFiles_Ins", new DbFile { FileName = filename, FileContent = content });

        var file = await DbHelper.ExecAsync<DbFile>("WJbFiles_Get", guid);

        Assert.Equal(filename, file?.FileName);

        Assert.Equal(content, file?.FileContent);

        await DbHelper.ExecAsync<DbFile>("WJbFiles_Del", guid);
    }
}