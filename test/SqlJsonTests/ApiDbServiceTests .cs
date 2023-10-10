// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json.Nodes;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class ApiDbServiceTests
{
    private readonly IDbService _db;

    public ApiDbServiceTests()
    {
        _db = new ApiDbService(Http);
    }

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<byte>() },
            new object[] { TestBytes1k },
            //new object[] { TestBytes5k },
            //new object[] { TestBytes55k }
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


    [Fact]
    public async Task CanExecAsync()
    {
        Assert.Equal(-1, await _db.DeleteAsync("Exec0"));
        Assert.Equal(1, await _db.DeleteAsync("Exec1"));

        Assert.Null(await _db.ExecAsync<bool?>("Exec0"));

        Assert.Null(await _db.ExecAsync<bool?>("ProcNull"));

        Assert.Null(await _db.ExecAsync<bool?>("ProcVar", null));
        Assert.True(await _db.ExecAsync<bool>("ProcVar", true));
        Assert.False(await _db.ExecAsync<bool>("ProcVar", false));

        Assert.Equal(0, await _db.ExecAsync<int>("ProcVar", 0));
        Assert.Equal(byte.MaxValue, await _db.ExecAsync<byte>("ProcVar", byte.MaxValue));
        Assert.Equal(short.MaxValue, await _db.ExecAsync<short>("ProcVar", short.MaxValue));
        Assert.Equal(int.MaxValue, await _db.ExecAsync<int>("ProcVar", int.MaxValue));
        Assert.Equal(long.MaxValue, await _db.ExecAsync<long>("ProcVar", long.MaxValue));

        Assert.Equal(decimal.MaxValue, await _db.ExecAsync<decimal>("ProcVar", decimal.MaxValue));
        Assert.Equal(float.MaxValue, await _db.ExecAsync<float>("ProcVar", float.MaxValue));
        Assert.Equal(double.MaxValue, await _db.ExecAsync<double>("ProcVar", double.MaxValue));

        Assert.Equal(DateOnly.MaxValue, await _db.ExecAsync<DateOnly>("ProcVar", DateOnly.MaxValue));
        Assert.Equal(new DateTime(2000, 01, 13, 23, 0, 0), await _db.ExecAsync<DateTime>("ProcVar", new DateTime(2000, 01, 13, 23, 0, 0)));

        Assert.Equal(new DateTimeOffset(new DateTime(2000, 01, 13, 23, 0, 0)), await _db.ExecAsync<DateTimeOffset>("ProcVar", new DateTimeOffset(new DateTime(2000, 01, 13, 23, 0, 0))));
        Assert.Equal(new TimeOnly(23, 59, 59), await _db.ExecAsync<TimeOnly>("ProcVar", new TimeOnly(23, 59, 59)));
        //Assert.Equal(new TimeSpan(23, 59, 59), await _db.ExecAsync<TimeSpan>("ProcVar", new TimeSpan(23, 59, 59)));

        Assert.Equal(Guid.Empty, await _db.ExecAsync<Guid>("ProcVar", Guid.Empty));

        Assert.Equal('x', await _db.ExecAsync<char>("ProcVar", 'x'));
        Assert.Equal(string.Empty, await _db.ExecAsync<string>("ProcVar", string.Empty));
        Assert.Equal("asd asd", await _db.ExecAsync<string>("ProcVar", "asd asd"));

        Assert.Equal(new byte[] { 0, 10, 100, byte.MaxValue }, await _db.ExecAsync<byte[]>("ProcVar", new byte[] { 0, 10, 100, byte.MaxValue }));
        Assert.Equal(new char[] { '1', '2', '3' }, await _db.ExecAsync<char[]>("ProcVarChar", new char[] { '1', '2', '3' }));

        Assert.Equal(UserType.User, await _db.ExecAsync<UserType?>("ProcVar", UserType.User));

        Assert.Equal("John", await _db.ExecAsync<string?>("ProcObj", new { Name = "John" }));

        var rec1 = await _db.ExecAsync<JsonObject>("ProcObj1");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = await _db.ExecAsync<List<JsonObject>>("ProcObj2");
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.Equal(1, (int?)recs[0]["Id"]);
        Assert.Equal("John", (string?)recs[0]["Name"]);
        Assert.Equal(2, (int?)recs[1]["Id"]);
        Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Bytes(byte[] bytes) => Assert.Equal(bytes, await _db.ExecAsync<byte[]?>("ProcVar", bytes));

    //[Theory]
    //[MemberData(nameof(GetTestBytes), parameters: 4)]
    //public async Task CanExecAsync_Stream(byte[] bytes)
    //{
    //    using var msIn = new MemoryStream(bytes);
    //    using var stream = await _db.ExecAsync<Stream>("ProcVarBin", msIn);

    //    Assert.NotNull(stream);
    //    Assert.Equal(bytes, Stream2Bytes(stream));

    //    byte[] Stream2Bytes(Stream input)
    //    {
    //        MemoryStream ms = new();
    //        input.CopyTo(ms);
    //        return ms.ToArray();
    //    }
    //}

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public async Task CanExecAsync_Chars(char[] chars) => Assert.Equal(chars, await _db.ExecAsync<char[]?>("ProcVarChar", chars));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_String(string str) => Assert.Equal(str, await _db.ExecAsync<string?>("ProcVarChar", str));

    //[Theory]
    //[MemberData(nameof(GetTestString), parameters: 4)]
    //public async Task CanExecAsync_TextReader(string text)
    //{
    //    using TextReader readerSource = new StringReader(text);
    //    using var readerResult = await _db.ExecAsync<TextReader>("ProcVarChar", readerSource);

    //    Assert.NotNull(readerResult);
    //    Assert.Equal(text, await readerResult.ReadToEndAsync());
    //}

    [Fact]
    public async Task CanCrudAsync()
    {
        var item1 = new { Name = "DbName1" };

        var id = await _db.CreateAsync<decimal?>("TestItems_Ins", item1);

        Assert.NotNull(id);

        var item2 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbName2";

        await _db.UpdateAsync("TestItems_Upd", item2);

        var item3 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await _db.DeleteAsync("TestItems_Del", id);

        var item4 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.Null(item4);
    }

    //[Fact]
    //public async Task CanTestAsync()
    //{
    //    await _db.TestAsync("InputVar_Zap");
    //}
}