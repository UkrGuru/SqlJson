// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.SqlJson;

public class DbHelperTests
{
    public DbHelperTests() => DbHelper.ConnectionString = GlobalTests.ConnectionString;

    //[Fact]
    //public static void CanCreateSqlConnection()
    //{
    //    var connection = DbHelper.CreateSqlConnection();

    //    Assert.NotNull(connection);
    //    Assert.Equal(Globals.DbName, connection.Database);
    //    Assert.Equal(Globals.ConnectionString, connection.ConnectionString);
    //}

    [Theory]
    [InlineData("Proc1", true)]
    [InlineData("[Proc1]", true)]
    [InlineData("dbo.Proc1", true)]
    [InlineData("[dbo].Proc1", true)]
    [InlineData("[Proc 1]", true)]
    [InlineData("SELECT 1; /* need more text here for CommandText type */", false)]
    public void IsNameTests(string cmdText, bool expected)
        => Assert.Equal(expected, DbHelper.IsName(cmdText));

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("Boolean", false)]
    [InlineData("Byte", false)]
    [InlineData("DateTime", false)]
    [InlineData("DateTimeOffset", false)]
    [InlineData("Decimal", false)]
    [InlineData("Double", false)]
    [InlineData("Guid", false)]
    [InlineData("Int16", false)]
    [InlineData("Int32", false)]
    [InlineData("Single", false)]
    [InlineData("TimeSpan", false)]
    [InlineData("Byte[]", true)]
    [InlineData("Char[]", true)]
    [InlineData("Xml", true)]
    public void IsLongTests(string? typeName, bool expected)
        => Assert.Equal(expected, DbHelper.IsLong(typeName));

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 16)]
    public void CanNormalize(object data, object expected)
        => Assert.Equal(expected, DbHelper.Normalize(data));

    public static IEnumerable<object[]> GetData4CanNormalize(int numTests)
    {
        var guid = Guid.NewGuid();
        string data = JsonSerializer.Serialize(new { Name = "Proc1" })!;
        var allData = new List<object[]>
        {
            new object[] { "str1", "str1" },
            new object[] { true, true },
            new object[] { false, false },
            new object[] { (byte)1, (byte)1 },
            new object[] { new byte[] { 1, 2 }, new byte[] { 1, 2 } },
            new object[] { new char[] { '1', '2' }, new char[] { '1', '2' } },
            new object[] { 123.45d, 123.45d },
            new object[] { 123.45f, 123.45f },
            new object[] { 123, 123 },
            new object[] { 12345, 12345 },
            new object[] { 1234567890, 1234567890 },
            new object[] { new DateTime(2000, 1, 1), new DateTime(2000, 1, 1) },
            new object[] { new DateTimeOffset(new DateTime(2000, 1, 1)), new DateTimeOffset(new DateTime(2000, 1, 1)) },
            new object[] { guid, guid },
            new object[] { new { Name = "Proc1" }, data },
            new object[] { JsonSerializer.Deserialize<dynamic?>(data)!, data }
        };

        return allData.Take(numTests);
    }

    [Fact]
    public void CanExec()
    {
        var num1 = DbHelper.Exec("DECLARE @num1 int; SET @num1 = 1; /* need more text here for CommandText type */");
        Assert.Equal(-1, num1);

        var num2 = DbHelper.Exec<int?>("SELECT CAST(1 as varchar); /* need more text here for CommandText type */");
        Assert.Equal(1, num2);

        var num3 = DbHelper.Exec<int?>("SELECT CAST(NULL as varchar); /* need more text here for CommandText type */");
        Assert.Null(num3);

        var data = DbHelper.Exec<string?>("SELECT @Data; /* need more text here for CommandText type */", "Data");
        Assert.Equal("Data", data);

        var name = DbHelper.Exec<string?>("SELECT JSON_VALUE(@Data, '$.Name'); /* need more text here for CommandText type */", new { Name = "John" });
        Assert.Equal("John", name);

        DbHelper.Exec("CREATE OR ALTER PROCEDURE ProcNull AS /* need more text here for CommandText type */");

        DbHelper.Exec("CREATE OR ALTER PROCEDURE ProcInt @Data int = NULL AS SELECT CAST(@Data as varchar);");

        DbHelper.Exec("CREATE OR ALTER PROCEDURE ProcStr @Data varchar(100) = NULL AS SELECT @Data;");

        DbHelper.Exec("CREATE OR ALTER PROCEDURE ProcObj @Data varchar(100) = NULL AS SELECT JSON_VALUE(@Data, '$.Name');");

        DbHelper.Exec("ProcNull");

        var num4 = DbHelper.Exec<int?>("ProcInt", 1);
        Assert.Equal(1, num4);

        var num5 = DbHelper.Exec<int?>("ProcInt", null);
        Assert.Null(num5);

        var data2 = DbHelper.Exec<string?>("ProcStr", "Data");
        Assert.Equal("Data", data2);

        var name2 = DbHelper.Exec<string?>("ProcObj", new { Name = "John" });
        Assert.Equal("John", name2);
    }

    [Fact]
    public async Task CanExecAsync()
    {
        var num1 = await DbHelper.ExecAsync("DECLARE @num1 int; SET @num1 = 1; /* need more text here for CommandText type */");
        Assert.Equal(-1, num1);

        var num2 = await DbHelper.ExecAsync<int?>("SELECT CAST(1 as varchar); /* need more text here for CommandText type */");
        Assert.Equal(1, num2);

        var num3 = await DbHelper.ExecAsync<int?>("SELECT CAST(NULL as varchar); /* need more text here for CommandText type */");
        Assert.Null(num3);

        var data = await DbHelper.ExecAsync<string?>("SELECT @Data; /* need more text here for CommandText type */", "Data");
        Assert.Equal("Data", data);

        var name = await DbHelper.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name'); /* need more text here for CommandText type */", new { Name = "John" });
        Assert.Equal("John", name);

        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE ProcNullAsync AS /* need more text here for CommandText type */");

        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE ProcIntAsync @Data int = NULL AS SELECT CAST(@Data as varchar);");

        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE ProcStrAsync @Data varchar(100) = NULL AS SELECT @Data;");

        await DbHelper.ExecAsync("CREATE OR ALTER PROCEDURE ProcObjAsync @Data varchar(100) = NULL AS SELECT JSON_VALUE(@Data, '$.Name');");

        await DbHelper.ExecAsync("ProcNullAsync");

        var num4 = await DbHelper.ExecAsync<int?>("ProcIntAsync", 1);
        Assert.Equal(1, num4);

        var num5 = await DbHelper.ExecAsync<int?>("ProcIntAsync", null);
        Assert.Null(num5);

        var data2 = await DbHelper.ExecAsync<string?>("ProcStrAsync", "Data");
        Assert.Equal("Data", data2);

        var name2 = await DbHelper.ExecAsync<string?>("ProcObjAsync", new { Name = "John" });
        Assert.Equal("John", name2);
    }
}