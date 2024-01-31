// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data.SqlTypes;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Tests;

public class ApiDbServiceTests
{
    private readonly IDbService _db;

    public ApiDbServiceTests() => _db = new ApiDbService(Http);

    public static IEnumerable<object[]> GetTestBytes(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { Array.Empty<byte>() },
            new object[] { TestBytes1k },
            new object[] { TestBytes5k },
            new object[] { TestBytes55k },
        };

        return allData.Take(numTests);
    }

    public static IEnumerable<object[]> GetTestChars(int numTests)
    {
        var allData = new List<object[]>
        {
            new object[] { TestChars1k },
            new object[] { TestChars5k },
            new object[] { TestChars55k },
            new object[] { Array.Empty<char>() },
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
    public async Task CanExecAsync_Null()
    {
        Assert.Equal(-1, await _db.DeleteAsync("Exec0"));
        Assert.Equal(1, await _db.DeleteAsync("Exec1"));

        Assert.Null(await _db.ExecAsync<bool?>("Exec0"));
        Assert.Null(await _db.ExecAsync<bool?>("ProcNull"));

        Assert.Null(await _db.ExecAsync<bool?>("ProcVar", null));
        Assert.Null(await _db.ExecAsync<bool?>("ProcVar", DBNull.Value));

        Assert.Null(await _db.ExecAsync<byte?>("ProcVar", SqlByte.Null));
        Assert.Null(await _db.ExecAsync<short?>("ProcVar", SqlInt16.Null));
        Assert.Null(await _db.ExecAsync<int?>("ProcVar", SqlInt32.Null));
        Assert.Null(await _db.ExecAsync<long?>("ProcVar", SqlInt64.Null));
        Assert.Null(await _db.ExecAsync<decimal?>("ProcVar", SqlDecimal.Null));
        Assert.Null(await _db.ExecAsync<float?>("ProcVar", SqlSingle.Null));
        Assert.Null(await _db.ExecAsync<double?>("ProcVar", SqlDouble.Null));
        Assert.Null(await _db.ExecAsync<decimal?>("ProcVar", SqlMoney.Null));

        Assert.Null(await _db.ExecAsync<DateTime?>("ProcVar", SqlDateTime.Null));

        Assert.Null(await _db.ExecAsync<bool?>("ProcVar", SqlBoolean.Null));
        Assert.Null(await _db.ExecAsync<Guid?>("ProcVar", SqlGuid.Null));

        Assert.Null(await _db.ExecAsync<byte[]?>("ProcVar", null));
        Assert.Null(await _db.ExecAsync<byte[]?>("ProcVar", SqlBinary.Null));
        Assert.Null(await _db.ExecAsync<byte[]?>("ProcVar", SqlBytes.Null));

        Assert.Empty((await _db.ExecAsync<string?>("ProcVar", SqlChars.Null))!);
        Assert.Empty((await _db.ExecAsync<string?>("ProcVar", SqlString.Null))!);

        Assert.Empty((await _db.ExecAsync<string?>("ProcXml", SqlXml.Null))!);
    }

    [Fact]
    public async Task CanExecAsync_Boolean()
    {
        Assert.True(await _db.ExecAsync<bool>("ProcVar", true));
        Assert.True(await _db.ExecAsync<bool>("ProcVar", SqlBoolean.True));

        Assert.False(await _db.ExecAsync<bool>("ProcVar", false));
        Assert.False(await _db.ExecAsync<bool>("ProcVar", SqlBoolean.False));
    }

    [Fact]
    public async Task CanExecAsync_Numeric()
    {
        object? value = null, sqlValue = null;

        value = byte.MinValue; Assert.Equal(value, await _db.ExecAsync<byte>("ProcVar", value));
        sqlValue = new SqlByte((byte)value); Assert.Equal(value, await _db.ExecAsync<byte>("ProcVar", sqlValue));

        value = byte.MaxValue; Assert.Equal(value, await _db.ExecAsync<byte>("ProcVar", value));
        sqlValue = new SqlByte((byte)value); Assert.Equal(value, await _db.ExecAsync<byte>("ProcVar", sqlValue));

        value = short.MaxValue; Assert.Equal(value, await _db.ExecAsync<short>("ProcVar", value));
        sqlValue = new SqlInt16((short)value); Assert.Equal(value, await _db.ExecAsync<short>("ProcVar", sqlValue));

        value = int.MaxValue; Assert.Equal(value, await _db.ExecAsync<int>("ProcVar", value));
        sqlValue = new SqlInt32((int)value); Assert.Equal(value, await _db.ExecAsync<int>("ProcVar", sqlValue));

        value = long.MaxValue; Assert.Equal(value, await _db.ExecAsync<long>("ProcVar", value));
        sqlValue = new SqlInt64((long)value); Assert.Equal(value, await _db.ExecAsync<long>("ProcVar", sqlValue));

        value = decimal.MaxValue; Assert.Equal(value, await _db.ExecAsync<decimal>("ProcVar", value));
        sqlValue = new SqlDecimal((decimal)value); Assert.Equal(value, await _db.ExecAsync<decimal>("ProcVar", sqlValue));

        value = float.MaxValue; Assert.Equal(value, await _db.ExecAsync<float>("ProcVar", value));
        sqlValue = new SqlSingle((float)value); Assert.Equal(value, await _db.ExecAsync<float>("ProcVar", sqlValue));

        value = double.MaxValue; Assert.Equal(value, await _db.ExecAsync<double>("ProcVar", value));
        sqlValue = new SqlDouble((double)value); Assert.Equal(value, await _db.ExecAsync<double>("ProcVar", sqlValue));

        sqlValue = new SqlMoney(45m); Assert.Equal(45m, await _db.ExecAsync<decimal>("ProcVar", sqlValue));
    }

    [Fact]
    public async Task CanExecAsync_DateTime()
    {
        object? value = null, sqlValue = null;

        value = new TimeSpan(0, 23, 59, 59, 999, 999);
        Assert.Equal(value, await _db.ExecAsync<TimeSpan>("ProcVar", value));

        value = DateOnly.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<DateOnly>("ProcVar", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, await _db.ExecAsync<DateTime>("ProcVar", value));
        Assert.Equal(value, await _db.ExecAsync<DateTime>("ProcVar", sqlValue));

        value = new DateTimeOffset((DateTime)value);
        Assert.Equal(value, await _db.ExecAsync<DateTimeOffset>("ProcVar", value));

        value = new TimeOnly(23, 59, 59);
        Assert.Equal(value, await _db.ExecAsync<TimeOnly>("ProcVar", value));

    }

    [Fact]
    public async Task CanExecAsync_Other()
    {
        object? value = null, sqlValue = null;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, await _db.ExecAsync<Guid>("ProcVar", value));
        Assert.Equal(value, await _db.ExecAsync<Guid>("ProcVar", sqlValue));

        value = 'V';
        Assert.Equal(value, await _db.ExecAsync<char>("ProcVar", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await _db.ExecAsync<string>("ProcVar", value));
        Assert.Equal(value, await _db.ExecAsync<string>("ProcVar", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await _db.ExecAsync<string>("ProcVar", value));
        Assert.Equal(value, await _db.ExecAsync<string>("ProcVar", sqlValue));

        Assert.Equal(UserType.User, await _db.ExecAsync<UserType?>("ProcVar", UserType.User));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Bytes(byte[] bytes)
        => Assert.Equal(bytes, await _db.ExecAsync<byte[]?>("ProcVarBin", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_SqlBinary(byte[] bytes)
        => Assert.Equal(bytes, await _db.ExecAsync<byte[]?>("ProcVarBin", new SqlBinary(bytes)));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_SqlBytes(byte[] bytes)
        => Assert.Equal(bytes, await _db.ExecAsync<byte[]?>("ProcVarBin", new SqlBytes(bytes)));

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
    [MemberData(nameof(GetTestChars), parameters: 3)]
    public async Task CanExecAsync_Chars(char[] chars)
        => Assert.Equal(chars, await _db.ExecAsync<char[]?>("ProcVarChar", chars));

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 3)]
    public async Task CanExecAsync_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(chars, await _db.ExecAsync<char[]?>("ProcVarChar", sqlValue));
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_String(string str)
        => Assert.Equal(str, await _db.ExecAsync<string?>("ProcVarChar", str));

    //[Theory]
    //[MemberData(nameof(GetTestString), parameters: 4)]
    //public async Task CanExecAsync_TextReader(string text)
    //{
    //    using TextReader readerSource = new StringReader(text);
    //    using var readerResult = await _db.ExecAsync<TextReader>("ProcVarChar", readerSource);

    //    Assert.NotNull(readerResult);
    //    Assert.Equal(text, await readerResult.ReadToEndAsync());
    //}

    //[Theory]
    //[MemberData(nameof(GetTestString), parameters: 4)]
    //public async Task CanExecAsync_SqlXml(string text)
    //{
    //    var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

    //    var sqlValue = new SqlXml(new System.Xml.XmlTextReader(new StringReader(value)));

    //    var sqlActual = await _db.ExecAsync<SqlXml>("ProcXml", sqlValue);

    //    Assert.NotNull(sqlActual);
    //    Assert.Equal(sqlActual.Value, sqlValue.Value);
    //}

    //[Theory]
    //[MemberData(nameof(GetTestString), parameters: 4)]
    //public async Task CanExecAsync_XmlReader(string text)
    //{
    //    var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

    //    using var readerIn = XmlReader.Create(new StringReader(value));

    //    using var readerOut = await _db.ExecAsync<XmlReader>("ProcXml", readerIn);

    //    Assert.NotNull(readerOut);

    //    readerOut.Read();

    //    Assert.Equal(value, await readerOut.ReadOuterXmlAsync());
    //}

    [Fact]
    public async Task CanExecAsync_Record()
    {
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

    [Fact]
    public async Task CanExecAsync_Crud()
    {
        var item1 = new { Name = "DbName1" };

        var id = await _db.CreateAsync<int?>("TestItems_Ins", item1);

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
}