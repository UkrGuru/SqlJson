// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Text.Json;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Tests;

public class DbExtensionsTests
{
    [Fact]
    public void IsLong_FactTests()
    {
        Assert.False(DbExtensions.IsLong<byte>());
        Assert.False(DbExtensions.IsLong<short>());
        Assert.False(DbExtensions.IsLong<int>());
        Assert.False(DbExtensions.IsLong<long>());
        Assert.False(DbExtensions.IsLong<float>());
        Assert.False(DbExtensions.IsLong<double>());
        Assert.False(DbExtensions.IsLong<decimal>());

        Assert.False(DbExtensions.IsLong<bool>());
        Assert.False(DbExtensions.IsLong<char>());
        Assert.False(DbExtensions.IsLong<Guid>());
        Assert.False(DbExtensions.IsLong<UserType>());

        Assert.False(DbExtensions.IsLong<DateOnly>());
        Assert.False(DbExtensions.IsLong<DateTime>());
        Assert.False(DbExtensions.IsLong<DateTimeOffset>());
        Assert.False(DbExtensions.IsLong<TimeOnly>());
        Assert.False(DbExtensions.IsLong<TimeSpan>());

        Assert.False(DbExtensions.IsLong<SqlByte>());
        Assert.False(DbExtensions.IsLong<SqlInt16>());
        Assert.False(DbExtensions.IsLong<SqlInt32>());
        Assert.False(DbExtensions.IsLong<SqlInt64>());
        Assert.False(DbExtensions.IsLong<SqlSingle>());
        Assert.False(DbExtensions.IsLong<SqlDecimal>());
        Assert.False(DbExtensions.IsLong<SqlMoney>());

        Assert.False(DbExtensions.IsLong<SqlBoolean>());
        Assert.False(DbExtensions.IsLong<SqlGuid>());

        Assert.True(DbExtensions.IsLong<string>());
        Assert.True(DbExtensions.IsLong<byte[]>());
        Assert.True(DbExtensions.IsLong<char[]>());
        Assert.True(DbExtensions.IsLong<Stream>());
        Assert.True(DbExtensions.IsLong<TextReader>());
        Assert.True(DbExtensions.IsLong<object>());

        Assert.True(DbExtensions.IsLong<SqlBinary>());
        Assert.True(DbExtensions.IsLong<SqlBytes>());
        Assert.True(DbExtensions.IsLong<SqlChars>());
        Assert.True(DbExtensions.IsLong<SqlString>());
        Assert.True(DbExtensions.IsLong<SqlXml>());
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(".", false)]
    [InlineData("SELECT 1", false)]

    [InlineData(" ", false)]
    [InlineData("1", false)]
    [InlineData("_", true)]
    [InlineData("a", true)]
    [InlineData("A", true)]
    [InlineData("_1", true)]
    [InlineData("a1", true)]
    [InlineData("A1", true)]
    [InlineData("A 1", false)]
    [InlineData("[ ]", true)]
    [InlineData("[1]", true)]
    [InlineData("[A 1]", true)]

    [InlineData(" .A", false)]
    [InlineData("_.A", true)]
    [InlineData("a.A", true)]
    [InlineData("A.A", true)]
    [InlineData("1.A", false)]
    [InlineData("_1.A", true)]
    [InlineData("a1.A", true)]
    [InlineData("A1.A", true)]
    [InlineData("[ ].A", true)]
    [InlineData("[1].A", true)]

    [InlineData("dbo. ", false)]
    [InlineData("dbo._", true)]
    [InlineData("dbo.a", true)]
    [InlineData("dbo.A", true)]
    [InlineData("dbo.1", false)]
    [InlineData("dbo._1", true)]
    [InlineData("dbo.a1", true)]
    [InlineData("dbo.A1", true)]
    [InlineData("dbo.[ ]", true)]
    [InlineData("dbo.[1]", true)]
    public void IsNameTests(string? cmdText, bool expected)
        => Assert.Equal(expected, DbExtensions.IsName(cmdText));

    [Theory]
    [InlineData("SELECT @Data", null)]
    [InlineData("SELECT @Data", 1)]
    [InlineData("SELECT @Data", null, 15)]
    [InlineData("SELECT @Data", null, 45)]
    [InlineData("ProcTest", null, null, CommandType.StoredProcedure)]
    public static void CanCreateSqlCommand(string cmdText, object? data = null, int? timeout = null, CommandType expected = CommandType.Text)
    {
        SqlConnection connection = new SqlConnection(ConnectionString);

        var command = connection.CreateSqlCommand(cmdText, data, timeout);

        Assert.NotNull(command);
        Assert.Equal(cmdText, command.CommandText);

        Assert.NotNull(command.Parameters);
        if (data == null)
        {
            Assert.Equal(0, command.Parameters.Count);
        }
        else
        {
            Assert.Equal(1, command.Parameters.Count);
            Assert.Equal("@Data", command.Parameters[0].ParameterName);
            Assert.Equal(data, command.Parameters[0].Value);
        }

        Assert.Equal(timeout ?? 30, command.CommandTimeout);

        Assert.Equal(expected, command.CommandType);
    }

    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        var objJohn = new { Name = "John" };

        var allData = new List<object[]>
        {
            new object[] { true, SqlBoolean.True },
            new object[] { SqlBoolean.True, SqlBoolean.True },
            new object[] { false, SqlBoolean.False },
            new object[] { SqlBoolean.False, SqlBoolean.False },

            new object[] { byte.MinValue, SqlByte.MinValue },
            new object[] { SqlByte.MinValue, SqlByte.MinValue },
            new object[] { SqlByte.MaxValue, SqlByte.MaxValue },
            new object[] { short.MaxValue, SqlInt16.MaxValue },
            new object[] { SqlInt16.MaxValue, SqlInt16.MaxValue },
            new object[] { int.MaxValue, SqlInt32.MaxValue },
            new object[] { SqlInt32.MaxValue, SqlInt32.MaxValue },
            new object[] { long.MaxValue, SqlInt64.MaxValue },
            new object[] { SqlInt64.MaxValue, SqlInt64.MaxValue },
            new object[] { decimal.MaxValue, new SqlDecimal(decimal.MaxValue) },
            new object[] { SqlDecimal.MaxValue, SqlDecimal.MaxValue },
            new object[] { float.MaxValue, SqlSingle.MaxValue },
            new object[] { SqlSingle.MaxValue, SqlSingle.MaxValue },
            new object[] { double.MaxValue, SqlDouble.MaxValue },
            new object[] { SqlDouble.MaxValue, SqlDouble.MaxValue },
            new object[] { new SqlMoney(45m), new SqlMoney(45m) },

            new object[] { new DateOnly(2000, 11, 25), new DateOnly(2000, 11, 25) },
            new object[] { DateTime.MaxValue, new SqlDateTime(DateTime.MaxValue) },
            new object[] { SqlDateTime.MaxValue, SqlDateTime.MaxValue },
            new object[] { DateTimeOffset.MaxValue, DateTimeOffset.MaxValue },
            new object[] { TimeOnly.MaxValue, TimeOnly.MaxValue },
            new object[] { TimeSpan.MaxValue, TimeSpan.MaxValue },

            new object[] { Guid.Empty, new SqlGuid(Guid.Empty) },
            new object[] { new SqlGuid(Guid.Empty), new SqlGuid(Guid.Empty) },
            new object[] { UserType.User, UserType.User },

            new object[] { 'V', new SqlString("V") },
            new object[] { string.Empty, new SqlString(string.Empty) },
            new object[] { new SqlString(string.Empty), new SqlString(string.Empty) },
            new object[] { "A V", new SqlString("A V") },
            new object[] { new SqlString("A V"), new SqlString("A V") },

            new object[] { objJohn, new SqlString(JsonSerializer.Serialize(objJohn)) },
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 40)]
    public void CanAddDataParameter(object data, object expected)
    {
        SqlCommand command = new SqlCommand();

        Assert.Equal(0, command.Parameters.Count);

        var result = command.Parameters.AddData(data);

        Assert.Equal(1, command.Parameters.Count);
        Assert.Equal("@Data", result.ParameterName);

        if (expected is Enum)
        {
            Assert.Equal(expected, result.Value);
        }
        else
        {
            Assert.Equal(expected, result.SqlValue);
        }
    }
}