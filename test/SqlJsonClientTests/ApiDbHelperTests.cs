// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data.SqlTypes;
using System.Text.Json;
using UkrGuru.SqlJson.Extensions;
using static UkrGuru.SqlJson.Client.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Client.Tests;

public class ApiDbHelperTests
{
    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        var objJohn = new { Name = "John" };
        var strJohn = JsonSerializer.Serialize(objJohn);

        var allData = new List<object[]>
        {
            new object[] { true, "True" },
            new object[] { false, "False" },

            new object[] { byte.MinValue, SqlByte.MinValue.ToString() },
            new object[] { short.MaxValue, SqlInt16.MaxValue.ToString() },
            new object[] { int.MaxValue, SqlInt32.MaxValue.ToString() },
            new object[] { long.MaxValue, SqlInt64.MaxValue.ToString() },
            new object[] { 45.1234m, "45.1234" },
            new object[] { float.MaxValue, SqlSingle.MaxValue.ToString() },
            new object[] { double.MaxValue, SqlDouble.MaxValue.ToString() },

            new object[] { new DateOnly(2000, 11, 25), "2000-11-25 00:00:00.000" },
            new object[] { new DateTime(2000, 11, 25, 13, 1, 1), "2000-11-25 13:01:01.000" },
            new object[] { new DateTimeOffset(2000, 11, 25, 13, 1, 1, TimeSpan.Zero), "2000-11-25 13:01:01.0000000 +00:00" },
            new object[] { new TimeOnly(13, 1, 1, 1),  "13:01:01" },
            new object[] { new TimeSpan(13, 1, 1), "13:01:01" },

            new object[] { Guid.Empty, "00000000-0000-0000-0000-000000000000" },
            new object[] { 'V', "V" },
            new object[] { string.Empty, "" },
            new object[] { "A V", "A V" },

            new object[] { new byte[] { 0, 10, 100, byte.MaxValue }, "0x000A64FF" },
            new object[] { new char[] { '1', '2', '3' }, "123" },

            new object[] { UserType.User, "1" },

            new object[] { objJohn, strJohn },
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 40)]
    public void CanNormalize(object data, string expected)
        => Assert.Equal(expected, ApiDbHelper.Normalize(data));

    [Theory]
    [InlineData(null, "[1]", null, null, "%5B1%5D")]
    [InlineData(null, "[1]", null, 100, "%5B1%5D?timeout=100")]
    [InlineData(null, "[1]", "", null, "%5B1%5D?data=")]
    [InlineData(null, "[1]", "", 100, "%5B1%5D?data=&timeout=100")]
    [InlineData(null, "[1]", "[1]", null, "%5B1%5D?data=%5B1%5D")]
    [InlineData("", "Proc1", null, null, "Proc1")]
    [InlineData("", "Proc1", "", null, "Proc1?data=")]
    [InlineData("", "Proc1", "[1]", null, "Proc1?data=%5B1%5D")]
    [InlineData("https://ApiHole", "Proc1", null, null, "https://ApiHole/Proc1")]
    [InlineData("https://ApiHole", "Proc1", "", null, "https://ApiHole/Proc1?data=")]
    [InlineData("https://ApiHole", "Proc1", "[1]", null, "https://ApiHole/Proc1?data=%5B1%5D")]
    public void CanNormalizeUrl(string? apiHoleUri, string proc, string? norm = default, int? timeout = default, string? expected = null)
        => Assert.Equal(expected, ApiDbHelper.Normalize(apiHoleUri, proc, norm, timeout));
}