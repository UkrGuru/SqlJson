// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using System.Text.Json.Nodes;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class DbHelperTests
{
    public DbHelperTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

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

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<byte>() },
            new object[] { TestBytes1k },
            new object[] { TestBytes5k },
            new object[] { TestBytes55k }
        };

        return allData.Take(numTests);
    }

    public static IEnumerable<object[]> GetTestChars(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<char>() },
            new object[] { TestChars1k },
            new object[] { TestChars5k },
            new object[] { TestChars55k }
        };

        return allData.Take(numTests);
    }

    public static IEnumerable<object[]> GetTestString(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { TestString1k },
            new object[] { TestString5k },
            new object[] { TestString55k }
        };

        return allData.Take(numTests);
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
        => Assert.Equal(expected, DbHelper.IsName(cmdText));

    [Theory]
    [MemberData(nameof(GetData4CanNormalize), parameters: 22)]
    public void CanNormalize(object data, object expected)
        => Assert.Equal(expected, DbHelper.Normalize(data));

    [Fact]
    public async Task CanExecAsync()
    {
        Assert.Equal(-1, await DbHelper.ExecAsync("DECLARE @Num0 int = 0"));
        Assert.Equal(1, await DbHelper.ExecAsync("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)"));

        Assert.Null(await DbHelper.ExecAsync<bool?>("DECLARE @Num0 int = 0"));

        Assert.Null(await DbHelper.ExecAsync<bool?>("SELECT NULL"));

        Assert.Null(await DbHelper.ExecAsync<bool?>("SELECT NULL", null));
        Assert.True(await DbHelper.ExecAsync<bool>("SELECT @Data", true));
        Assert.False(await DbHelper.ExecAsync<bool>("SELECT @Data", false));

        Assert.Equal(0, await DbHelper.ExecAsync<int>("SELECT @Data", 0));
        Assert.Equal(byte.MaxValue, await DbHelper.ExecAsync<byte>("SELECT @Data", byte.MaxValue));
        Assert.Equal(short.MaxValue, await DbHelper.ExecAsync<short>("SELECT @Data", short.MaxValue));
        Assert.Equal(int.MaxValue, await DbHelper.ExecAsync<int>("SELECT @Data", int.MaxValue));
        Assert.Equal(long.MaxValue, await DbHelper.ExecAsync<long>("SELECT @Data", long.MaxValue));

        Assert.Equal(decimal.MaxValue, await DbHelper.ExecAsync<decimal>("SELECT @Data", decimal.MaxValue));
        Assert.Equal(float.MaxValue, await DbHelper.ExecAsync<float>("SELECT @Data", float.MaxValue));
        Assert.Equal(double.MaxValue, await DbHelper.ExecAsync<double>("SELECT @Data", double.MaxValue));

        Assert.Equal(DateOnly.MaxValue, await DbHelper.ExecAsync<DateOnly>("SELECT @Data", DateOnly.MaxValue));
        Assert.Equal(new DateTime(2000, 01, 13, 23, 0, 0), await DbHelper.ExecAsync<DateTime>("SELECT @Data", new DateTime(2000, 01, 13, 23, 0, 0)));
        Assert.Equal(new DateTimeOffset(new DateTime(2000, 01, 13, 23, 0, 0)), await DbHelper.ExecAsync<DateTimeOffset>("SELECT @Data", new DateTimeOffset(new DateTime(2000, 01, 13, 23, 0, 0))));
        Assert.Equal(new TimeOnly(23, 59, 59), await DbHelper.ExecAsync<TimeOnly>("SELECT @Data", new TimeOnly(23, 59, 59)));
        Assert.Equal(new TimeSpan(23, 59, 59), await DbHelper.ExecAsync<TimeSpan>("SELECT @Data", new TimeSpan(23, 59, 59)));

        Assert.Equal(Guid.Empty, await DbHelper.ExecAsync<Guid>("SELECT @Data", Guid.Empty));

        Assert.Equal('x', await DbHelper.ExecAsync<char>("SELECT @Data", 'x'));
        Assert.Equal(string.Empty, await DbHelper.ExecAsync<string>("SELECT @Data", string.Empty));
        Assert.Equal("asd asd", await DbHelper.ExecAsync<string>("SELECT @Data", "asd asd"));

        Assert.Equal(new byte[] { 0, 10, 100, byte.MaxValue }, await DbHelper.ExecAsync<byte[]>("SELECT @Data", new byte[] { 0, 10, 100, byte.MaxValue }));
        Assert.Equal(new char[] { '1', '2', '3' }, await DbHelper.ExecAsync<char[]>("SELECT @Data", new char[] { '1', '2', '3' }));

        Assert.Equal(UserType.User, await DbHelper.ExecAsync<UserType?>("SELECT @Data", UserType.User));

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

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Bytes(byte[] bytes)
        => Assert.Equal(bytes, await DbHelper.ExecAsync<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Stream(byte[] bytes)
    {
        using var msIn = new MemoryStream(bytes);
        using var stream = await DbHelper.ExecAsync<Stream>("SELECT @Data", msIn);

        Assert.NotNull(stream);
        Assert.Equal(bytes, Stream2Bytes(stream));

        byte[] Stream2Bytes(Stream input)
        {
            MemoryStream ms = new();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public async Task CanExecAsync_Chars(char[] chars)
        => Assert.Equal(chars, await DbHelper.ExecAsync<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 3)]
    public async Task CanExecAsync_String(string str)
        => Assert.Equal(str, await DbHelper.ExecAsync<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 3)]
    public async Task CanExecAsync_TextReader(string text)
    {
        using TextReader readerSource = new StringReader(text);
        using var readerResult = await DbHelper.ExecAsync<TextReader>("SELECT @Data", readerSource);

        Assert.NotNull(readerResult);
        Assert.Equal(text, await readerResult.ReadToEndAsync());
    }

    [Fact]
    public async Task CanCrudAsync()
    {
        var item1 = new { Name = "DbHelperName1" };

        var id = await DbHelper.CreateAsync<decimal?>(@"
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(50))

SELECT SCOPE_IDENTITY()
", item1);

        Assert.NotNull(id);

        var item2 = await DbHelper.ReadAsync<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbHelperName2";

        await DbHelper.UpdateAsync(@"
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
", item2);

        var item3 = await DbHelper.ReadAsync<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await DbHelper.DeleteAsync(@"
DELETE TestItems
WHERE Id = @Data
", id);

        var item4 = await DbHelper.ReadAsync<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.Null(item4);
    }
}