// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson;

public class ApiHelperTests
{
    [Fact]
    public void CanNullNormalize() => Assert.Null(ApiHelper.Normalize(null));

    [Fact]
    public void CanStreamNormalize() => Assert.IsType<StreamContent>(ApiHelper.Normalize(new MemoryStream(GlobalTests.TestBytes1k)));

    [Fact]
    public void CanTextReaderNormalize()
    {
        using TextReader readerSource = new StringReader(GlobalTests.TestString1k);
        Assert.IsType<StringContent>(ApiHelper.Normalize(readerSource));
    }

    [Theory]
    [InlineData("")]
    [InlineData(true)]
    [InlineData((byte)1)]
    [InlineData(new byte[] { 1, 2 })]
    [InlineData(new char[] { '1', '2' })]
    [InlineData(123.45d)]
    [InlineData(123.45f)]
    [InlineData(123)]
    [InlineData(12345)]
    [InlineData(1234567890)]
    public void CanObjectNormalize(object? value) => Assert.IsType<StringContent>(ApiHelper.Normalize(value));

    [Theory]
    [InlineData(null, "Ins", null, "Ins")]
    [InlineData("", "Ins", "", "Ins?Data=")]
    [InlineData("ApiHole", "Ins", null, "ApiHole/Ins")]
    [InlineData("ApiHole", "Get", 1, "ApiHole/Get?Data=1")]
    [InlineData("ApiHole", "Get", "1", "ApiHole/Get?Data=1")]
    [InlineData("ApiHole", "Upd", null, "ApiHole/Upd")]
    [InlineData("ApiHole", "Del", 1, "ApiHole/Del?Data=1")]
    [InlineData("ApiHole", "Del", "1", "ApiHole/Del?Data=1")]
    public void CanNormalize(string? apiHoleUri, string proc, object? data = null, string? expected = null)
        => Assert.Equal(expected, ApiHelper.Normalize(apiHoleUri, proc, data));
}