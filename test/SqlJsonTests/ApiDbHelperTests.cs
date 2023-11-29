// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data.SqlTypes;
using System.Text.Json;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Tests;

public class ApiDbHelperTests
{
    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        var objJohn = new { Name = "John" };
        var strJohn = JsonSerializer.Serialize(objJohn);

        var allData = new List<object[]>
        {
            new object[] { true, "True" },
            new object[] { SqlBoolean.True, "True" },
            new object[] { false, "False" },
            new object[] { SqlBoolean.False, "False" },

            new object[] { byte.MinValue, SqlByte.MinValue.ToString() },
            new object[] { SqlByte.MinValue, SqlByte.MinValue.ToString() },
            new object[] { SqlByte.MaxValue, SqlByte.MaxValue.ToString() },
            new object[] { short.MaxValue, SqlInt16.MaxValue.ToString() },
            new object[] { SqlInt16.MaxValue, SqlInt16.MaxValue.ToString() },
            new object[] { int.MaxValue, SqlInt32.MaxValue.ToString() },
            new object[] { SqlInt32.MaxValue, SqlInt32.MaxValue.ToString() },
            new object[] { long.MaxValue, SqlInt64.MaxValue.ToString() },
            new object[] { SqlInt64.MaxValue, SqlInt64.MaxValue.ToString() },
            new object[] { 45.1234m, "45.1234" },
            new object[] { new SqlDecimal(45.1234m), "45.1234" },
            new object[] { float.MaxValue, SqlSingle.MaxValue.ToString() },
            new object[] { SqlSingle.MaxValue, SqlSingle.MaxValue.ToString() },
            new object[] { double.MaxValue, SqlDouble.MaxValue.ToString() },
            new object[] { SqlDouble.MaxValue, SqlDouble.MaxValue.ToString() },
            new object[] { new SqlMoney(45.1234m), "45.1234" },

            new object[] { new DateOnly(2000, 11, 25), "2000-11-25 00:00:00.000" },
            new object[] { new DateTime(2000, 11, 25, 13, 1, 1), "2000-11-25 13:01:01.000" },
            new object[] { new SqlDateTime(2000, 11, 25, 13, 1, 1), "2000-11-25 13:01:01.000" },
            new object[] { new DateTimeOffset(2000, 11, 25, 13, 1, 1, TimeSpan.Zero), "2000-11-25 13:01:01.0000000 +00:00" },
            new object[] { new TimeOnly(13, 1, 1, 1),  "13:01:01" },
            new object[] { new TimeSpan(13, 1, 1), "13:01:01" },

            new object[] { Guid.Empty, "00000000-0000-0000-0000-000000000000" },
            new object[] { new SqlGuid(Guid.Empty), "00000000-0000-0000-0000-000000000000" },
            new object[] { 'V', "V" },
            new object[] { string.Empty, "" },
            new object[] { new SqlString(string.Empty), "" },
            new object[] { "A V", "A V" },
            new object[] { new SqlString("A V"), "A V" },

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
    [InlineData(null, "[1]", null, "%5B1%5D")]
    [InlineData(null, "[1]", "", "%5B1%5D?Data=")]
    [InlineData(null, "[1]", "[1]", "%5B1%5D?Data=%5B1%5D")]
    [InlineData("", "Proc1", null, "Proc1")]
    [InlineData("", "Proc1", "", "Proc1?Data=")]
    [InlineData("", "Proc1", "[1]", "Proc1?Data=%5B1%5D")]
    [InlineData("https://ApiHole", "Proc1", null, "https://ApiHole/Proc1")]
    [InlineData("https://ApiHole", "Proc1", "", "https://ApiHole/Proc1?Data=")]
    [InlineData("https://ApiHole", "Proc1", "[1]", "https://ApiHole/Proc1?Data=%5B1%5D")]
    public void CanNormalizeUrl(string? apiHoleUri, string proc, string? norm = null, string? expected = null)
        => Assert.Equal(expected, ApiDbHelper.Normalize(apiHoleUri, proc, norm));
}