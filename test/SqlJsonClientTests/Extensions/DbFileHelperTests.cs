// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.Client.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbFileHelperTests
{
    public static readonly TheoryData<byte[]> GetTestBytes = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public static async Task CanZipFile(byte[] bytes)
    {
        var fileName = $"{DateTime.Now:HHmmss}.bin";

        DbFile file = new() { FileName = fileName, FileContent = bytes };

        await file.CompressAsync();

        if (file?.FileContent?.Length > 0)
        {
            Assert.EndsWith(".gzip", file.FileName);

            await file.DecompressAsync();

            Assert.Equal(fileName, file.FileName);

            Assert.Equal(bytes, file.FileContent);
        }
        else
        {
            Assert.Equal(fileName, file?.FileName);
        }
    }
}