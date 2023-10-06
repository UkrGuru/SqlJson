// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class ApiDbHelperTests
{
    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        string data = JsonSerializer.Serialize(new { Name = "Proc1" })!;
        var allData = new List<object[]>
        {
            new object[] { true, true },
            new object[] { false, false },
            new object[] { 0, 0 },
            new object[] { byte.MaxValue, byte.MaxValue },
            new object[] { short.MaxValue, short.MaxValue },
            new object[] { int.MaxValue, int.MaxValue },
            new object[] { long.MaxValue, long.MaxValue },
            new object[] { 0.0, 0.0 },
            new object[] { decimal.MaxValue, decimal.MaxValue },
            new object[] { float.MaxValue, float.MaxValue },
            new object[] { double.MaxValue, double.MaxValue },
            new object[] { DateOnly.MaxValue, DateOnly.MaxValue },
            new object[] { DateTime.MaxValue, DateTime.MaxValue },
            new object[] { DateTimeOffset.MaxValue, DateTimeOffset.MaxValue },
            new object[] { TimeOnly.MaxValue, TimeOnly.MaxValue },
            new object[] { TimeSpan.MaxValue, TimeSpan.MaxValue },
            new object[] { Guid.Empty, Guid.Empty },
            new object[] { 'x', 'x' },
            new object[] { string.Empty, string.Empty },
            new object[] { "asd asd", "asd asd" },
            new object[] { new byte[] { 0, 10, 100, byte.MaxValue }, new byte[] { 0, 10, 100, byte.MaxValue } },
            new object[] { new char[] { '1', '2', '3' }, new char[] { '1', '2', '3' } },
            new object[] { data, new { Name = "Proc1" } },
            new object[] { UserType.Guest, UserType.Guest }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [InlineData(null, "Get", null, "Get")]
    [InlineData("", "Get", null, "Get")]
    [InlineData("", "Get", "", "Get?Data=")]
    [InlineData("ApiHole", "Get", null, "ApiHole/Get")]
    [InlineData("ApiHole", "Get", "", "ApiHole/Get?Data=")]
    [InlineData("ApiHole", "Get", 1, "ApiHole/Get?Data=1")]
    [InlineData("ApiHole", "Get", "asd", "ApiHole/Get?Data=asd")]
    public void CanNormalizeUrl(string? apiHoleUri, string proc, object? data = null, string? expected = null)
        => Assert.Equal(expected, ApiDbHelper.Normalize(apiHoleUri, proc, data));
}