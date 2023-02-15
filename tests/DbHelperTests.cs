// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace UkrGuru.SqlJson;

public class DbHelperTests
{
    public DbHelperTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }
    //[Fact]
    //public static void CanCreateSqlConnection()
    //{
    //    var connection = DbHelper.CreateSqlConnection();

    //    Assert.NotNull(connection);
    //    Assert.Equal(Globals.DbName, connection.Database);
    //    Assert.Equal(Globals.ConnectionString, connection.ConnectionString);
    //}

    [Theory]
    [InlineData("Proc_1", true)]
    [InlineData("[Proc 1]", true)]
    [InlineData("[Proc 1 2]", true)]
    [InlineData("dbo_1.Proc_1", true)]
    [InlineData("dbo_1.[Proc 1]", true)]
    [InlineData("dbo_1.[Proc 1 2]", true)]
    [InlineData("[dbo_1].Proc_1", true)]
    [InlineData("[dbo_1].[Proc 1]", true)]
    [InlineData("[dbo_1].[Proc 1 2]", true)]
    [InlineData("[dbo 1].[Proc 1]", true)]
    [InlineData("[dbo 1].[Proc 1 2]", true)]
    [InlineData("SELECT 1", false)]
    public void IsNameTests(string cmdText, bool expected)
        => Assert.Equal(expected, DbHelper.IsName(cmdText));

    //[Theory]
    //[InlineData(null, true)]
    //[InlineData("", true)]
    //[InlineData(typeof(bool), false)]
    //[InlineData("Byte", false)]
    //[InlineData("DateTime", false)]
    //[InlineData("DateTimeOffset", false)]
    //[InlineData("Decimal", false)]
    //[InlineData("Double", false)]
    //[InlineData("Guid", false)]
    //[InlineData("Int16", false)]
    //[InlineData("Int32", false)]
    //[InlineData("Single", false)]
    //[InlineData("TimeSpan", false)]
    //[InlineData("Byte[]", true)]
    //[InlineData("Char[]", true)]
    //[InlineData("Xml", true)]
    //public void IsLongTests(Type t, bool expected)
    //    => Assert.Equal(expected, DbHelper.IsLong<t>());

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 19)]
    public void CanNormalize(object data, object expected) => Assert.Equal(expected, DbHelper.Normalize(data));

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
            new object[] { new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1) },
            new object[] { new DateTime(2000, 1, 1, 1, 1, 1), new DateTime(2000, 1, 1, 1, 1, 1) },
            new object[] { new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1)), new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1)) },
            new object[] { new TimeOnly(1, 1, 1), new TimeOnly(1, 1, 1) },
            new object[] { guid, guid },
            new object[] { new { Name = "Proc1" }, data },
            new object[] { JsonSerializer.Deserialize<dynamic?>(data)!, data }
        };

        return allData.Take(numTests);
    }

    [Fact]
    public void CanExec()
    {
        Assert.Equal(-1, DbHelper.Exec("DECLARE @Num1 int = 1"));

        Assert.Null(DbHelper.Exec<object?>("SELECT NULL"));

        Assert.Equal(true, DbHelper.Exec<bool?>("SELECT @Data", true));

        Assert.Equal(Guid.Empty, DbHelper.Exec<Guid?>("SELECT @Data", Guid.Empty));

        Assert.Equal('X', DbHelper.Exec<char?>("SELECT @Data", 'X'));

        Assert.Equal((byte)1, DbHelper.Exec<byte?>("SELECT @Data", (byte)1));
        Assert.Equal(1, DbHelper.Exec<int?>("SELECT @Data", 1));
        Assert.Equal((long)1, DbHelper.Exec<long?>("SELECT @Data", (long)1));
        Assert.Equal(1.0f, DbHelper.Exec<float?>("SELECT @Data", 1.0f));
        Assert.Equal(1.0d, DbHelper.Exec<double?>("SELECT @Data", 1.0d));
        Assert.Equal(1.0m, DbHelper.Exec<decimal?>("SELECT @Data", 1.0m));

        Assert.Equal(new DateOnly(2000, 1, 1), DbHelper.Exec<DateOnly?>("SELECT @Data", new DateOnly(2000, 1, 1)));
        Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1), DbHelper.Exec<DateTime?>("SELECT @Data", new DateTime(2000, 1, 1, 1, 1, 1)));
        Assert.Equal(new DateTimeOffset(new DateTime(2000, 1, 1)), DbHelper.Exec<DateTimeOffset?>("SELECT @Data", new DateTimeOffset(new DateTime(2000, 1, 1))));
        Assert.Equal(new TimeOnly(1, 1, 1), DbHelper.Exec<TimeOnly?>("SELECT @Data", new TimeOnly(1, 1, 1)));
        Assert.Equal(new TimeSpan(1, 1, 1), DbHelper.Exec<TimeSpan?>("SELECT @Data", new TimeSpan(1, 1, 1)));

        Assert.Equal("John", DbHelper.Exec<string?>("SELECT JSON_VALUE(@Data, '$.Name');", new { Name = "John" }));

        var rec1 = DbHelper.Exec<JsonObject>("SELECT 1 Id, 'John' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = DbHelper.Exec<List<JsonObject>>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name FOR JSON PATH");
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.Equal(1, (int?)recs[0]["Id"]);
        Assert.Equal("John", (string?)recs[0]["Name"]);
        Assert.Equal(2, (int?)recs[1]["Id"]);
        Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    [Fact]
    public async Task CanExecAsync()
    {
        Assert.Equal(-1, await DbHelper.ExecAsync("DECLARE @Num1 int = 1"));

        Assert.Null(await DbHelper.ExecAsync<object?>("SELECT NULL"));

        Assert.Equal(true, await DbHelper.ExecAsync<bool?>("SELECT @Data", true));

        Assert.Equal(Guid.Empty, await DbHelper.ExecAsync<Guid?>("SELECT @Data", Guid.Empty));

        Assert.Equal('X', await DbHelper.ExecAsync<char?>("SELECT @Data", 'X'));

        Assert.Equal((byte)1, await DbHelper.ExecAsync<byte?>("SELECT @Data", (byte)1));
        Assert.Equal(1, await DbHelper.ExecAsync<int?>("SELECT @Data", 1));
        Assert.Equal((long)1, await DbHelper.ExecAsync<long?>("SELECT @Data", (long)1));
        Assert.Equal(1.0f, await DbHelper.ExecAsync<float?>("SELECT @Data", 1.0f));
        Assert.Equal(1.0d, await DbHelper.ExecAsync<double?>("SELECT @Data", 1.0d));
        Assert.Equal(1.0m, await DbHelper.ExecAsync<decimal?>("SELECT @Data", 1.0m));

        Assert.Equal(new DateOnly(2000, 1, 1), await DbHelper.ExecAsync<DateOnly?>("SELECT @Data", new DateOnly(2000, 1, 1)));
        Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1), await DbHelper.ExecAsync<DateTime?>("SELECT @Data", new DateTime(2000, 1, 1, 1, 1, 1)));
        Assert.Equal(new DateTimeOffset(new DateTime(2000, 1, 1)), await DbHelper.ExecAsync<DateTimeOffset?>("SELECT @Data", new DateTimeOffset(new DateTime(2000, 1, 1))));
        Assert.Equal(new TimeOnly(1, 1, 1), await DbHelper.ExecAsync<TimeOnly?>("SELECT @Data", new TimeOnly(1, 1, 1)));
        Assert.Equal(new TimeSpan(1, 1, 1), await DbHelper.ExecAsync<TimeSpan?>("SELECT @Data", new TimeSpan(1, 1, 1)));

        Assert.Equal("John", await DbHelper.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name');", new { Name = "John" }));

        var rec1 = await DbHelper.ExecAsync<JsonObject>("SELECT 1 Id, 'John' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = await DbHelper.ExecAsync<List<JsonObject>>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name FOR JSON PATH");
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.Equal(1, (int?)recs[0]["Id"]);
        Assert.Equal("John", (string?)recs[0]["Name"]);
        Assert.Equal(2, (int?)recs[1]["Id"]);
        Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<byte>() },
            new object[] { GlobalTests.TestBytes1k },
            new object[] { GlobalTests.TestBytes5k },
            new object[] { GlobalTests.TestBytes5m }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_Bytes(byte[] bytes) => Assert.Equal(bytes, DbHelper.Exec<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Bytes(byte[] bytes) => Assert.Equal(bytes, await DbHelper.ExecAsync<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_Stream(byte[] bytes)
    {
        using var msIn = new MemoryStream(bytes);
        using var stream = DbHelper.Exec<Stream>("SELECT @Data", msIn);

        Assert.NotNull(stream);
        Assert.Equal(bytes, Stream2Bytes(stream));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Stream(byte[] bytes)
    {
        using var msIn = new MemoryStream(bytes);
        using var stream = await DbHelper.ExecAsync<Stream>("SELECT @Data", msIn);

        Assert.NotNull(stream);
        Assert.Equal(bytes, Stream2Bytes(stream));
    }

    private static byte[] Stream2Bytes(Stream input)
    {
        MemoryStream ms = new();
        input.CopyTo(ms);
        return ms.ToArray();
    }

    public static IEnumerable<object[]> GetTestChars(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<char>() },
            new object[] { GlobalTests.TestChars1k },
            new object[] { GlobalTests.TestChars5k },
            new object[] { GlobalTests.TestChars5m }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public void CanExec_Chars(char[] chars) => Assert.Equal(chars, DbHelper.Exec<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public async Task CanExecAsync_Chars(char[] chars) => Assert.Equal(chars, await DbHelper.ExecAsync<char[]?>("SELECT @Data", chars));

    public static IEnumerable<object[]> GetTestString(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { GlobalTests.TestString1k },
            new object[] { GlobalTests.TestString5k },
            new object[] { GlobalTests.TestString5m }
        };

        return allData.Take(numTests);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 3)]
    public void CanExec_String(string str) => Assert.Equal(str, DbHelper.Exec<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 3)]
    public async Task CanExecAsync_String(string str) => Assert.Equal(str, await DbHelper.ExecAsync<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public void CanExec_TextReader(string text)
    {
        using TextReader readerSource = new StringReader(text);
        using var readerResult = DbHelper.Exec<TextReader>("SELECT @Data", readerSource);

        Assert.NotNull(readerResult);
        Assert.Equal(text, readerResult.ReadToEnd());
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_TextReader(string text)
    {
        using TextReader readerSource = new StringReader(text);
        using var readerResult = await DbHelper.ExecAsync<TextReader>("SELECT @Data", readerSource);

        Assert.NotNull(readerResult);
        Assert.Equal(text, await readerResult.ReadToEndAsync());
    }

    [Fact]
    public void CanExec_StorProc()
    {
        Assert.Equal(-1, DbHelper.Exec("ProcNull"));

        Assert.Equal(1, DbHelper.Exec<int?>("ProcInt", 1));

        Assert.Null(DbHelper.Exec<int?>("ProcInt", null));

        Assert.Equal("Data", DbHelper.Exec<string?>("ProcStr", "Data"));

        Assert.Equal("John", DbHelper.Exec<string?>("ProcObj", new { Name = "John" }));
    }

    [Fact]
    public async Task CanExecAsync_StorProc()
    {
        Assert.Equal(-1, await DbHelper.ExecAsync("ProcNull"));

        Assert.Equal(1, await DbHelper.ExecAsync<int?>("ProcInt", 1));

        Assert.Null(await DbHelper.ExecAsync<int?>("ProcInt", null));

        Assert.Equal("Data", await DbHelper.ExecAsync<string?>("ProcStr", "Data"));

        Assert.Equal("John", await DbHelper.ExecAsync<string?>("ProcObj", new { Name = "John" }));
    }
}