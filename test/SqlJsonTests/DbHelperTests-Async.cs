// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data.SqlTypes;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Linq;
using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Tests;

public partial class DbHelperTests
{
    [Fact]
    public async Task CanExecAsync_Null()
    {
        Assert.Equal(-1, await DbHelper.DeleteAsync("DECLARE @Num0 int = 0"));
        Assert.Equal(1, await DbHelper.DeleteAsync("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)"));

        Assert.Null(await DbHelper.ExecAsync<bool?>("DECLARE @Num0 int = 0"));
        Assert.Null(await DbHelper.ExecAsync<bool?>("SELECT NULL"));

        Assert.Null(await DbHelper.ExecAsync<bool?>("SELECT @Data", DBNull.Value));

        Assert.Null(await DbHelper.ExecAsync<byte?>("SELECT @Data", SqlByte.Null));
        Assert.Null(await DbHelper.ExecAsync<short?>("SELECT @Data", SqlInt16.Null));
        Assert.Null(await DbHelper.ExecAsync<int?>("SELECT @Data", SqlInt32.Null));
        Assert.Null(await DbHelper.ExecAsync<long?>("SELECT @Data", SqlInt64.Null));
        Assert.Null(await DbHelper.ExecAsync<decimal?>("SELECT @Data", SqlDecimal.Null));
        Assert.Null(await DbHelper.ExecAsync<float?>("SELECT @Data", SqlSingle.Null));
        Assert.Null(await DbHelper.ExecAsync<double?>("SELECT @Data", SqlDouble.Null));
        Assert.Null(await DbHelper.ExecAsync<decimal?>("SELECT @Data", SqlMoney.Null));

        Assert.Null(await DbHelper.ExecAsync<DateTime?>("SELECT @Data", SqlDateTime.Null));

        Assert.Null(await DbHelper.ExecAsync<bool?>("SELECT @Data", SqlBoolean.Null));
        Assert.Null(await DbHelper.ExecAsync<Guid?>("SELECT @Data", SqlGuid.Null));

        Assert.Null(await DbHelper.ExecAsync<byte[]?>("SELECT @Data", SqlBinary.Null));
        Assert.Null(await DbHelper.ExecAsync<byte[]?>("SELECT @Data", SqlBytes.Null));

        Assert.Null(await DbHelper.ExecAsync<char[]?>("SELECT @Data", SqlChars.Null));
        Assert.Null(await DbHelper.ExecAsync<string?>("SELECT @Data", SqlString.Null));

        Assert.Null(await DbHelper.ExecAsync<string?>("SELECT @Data", SqlXml.Null));
    }

    [Fact]
    public async Task CanExecAsync_Boolean()
    {
        Assert.True(await DbHelper.ExecAsync<bool>("SELECT @Data", true));
        Assert.True(await DbHelper.ExecAsync<bool>("SELECT @Data", SqlBoolean.True));

        Assert.False(await DbHelper.ExecAsync<bool>("SELECT @Data", false));
        Assert.False(await DbHelper.ExecAsync<bool>("SELECT @Data", SqlBoolean.False));
    }

    [Fact]
    public async Task CanExecAsync_Numeric()
    {
        object? value = null, sqlValue = null;

        value = byte.MinValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, await DbHelper.ExecAsync<byte>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<byte>("SELECT @Data", sqlValue));

        value = byte.MaxValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, await DbHelper.ExecAsync<byte>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<byte>("SELECT @Data", sqlValue));

        value = short.MaxValue; sqlValue = new SqlInt16((short)value);
        Assert.Equal(value, await DbHelper.ExecAsync<short>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<short>("SELECT @Data", sqlValue));

        value = int.MaxValue; sqlValue = new SqlInt32((int)value);
        Assert.Equal(value, await DbHelper.ExecAsync<int>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<int>("SELECT @Data", sqlValue));

        value = long.MaxValue; sqlValue = new SqlInt64((long)value);
        Assert.Equal(value, await DbHelper.ExecAsync<long>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<long>("SELECT @Data", sqlValue));

        value = decimal.MaxValue; sqlValue = new SqlDecimal((decimal)value);
        Assert.Equal(value, await DbHelper.ExecAsync<decimal>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<decimal>("SELECT @Data", sqlValue));

        value = float.MaxValue; sqlValue = new SqlSingle((float)value);
        Assert.Equal(value, await DbHelper.ExecAsync<float>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<float>("SELECT @Data", sqlValue));

        value = double.MaxValue; sqlValue = new SqlDouble((double)value);
        Assert.Equal(value, await DbHelper.ExecAsync<double>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<double>("SELECT @Data", sqlValue));

        value = 45m; sqlValue = new SqlMoney((decimal)value);
        Assert.Equal(value, await DbHelper.ExecAsync<decimal>("SELECT @Data", sqlValue));
    }

    [Fact]
    public async Task CanExecAsync_DateTime()
    {
        object? value = null, sqlValue = null;

        value = DateOnly.MaxValue; 
        Assert.Equal(value, await DbHelper.ExecAsync<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, await DbHelper.ExecAsync<DateTime>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<DateTime>("SELECT @Data", sqlValue));

        value = new DateTimeOffset((DateTime)value); 
        Assert.Equal(value, await DbHelper.ExecAsync<DateTimeOffset>("SELECT @Data", value));
        
        value = new TimeOnly(23, 59, 59); 
        Assert.Equal(value, await DbHelper.ExecAsync<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999, 999);
        Assert.Equal(value, await DbHelper.ExecAsync<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public async Task CanExecAsync_Other()
    {
        object? value = null, sqlValue = null;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, await DbHelper.ExecAsync<Guid>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<Guid>("SELECT @Data", sqlValue));

        value = 'V'; 
        Assert.Equal(value, await DbHelper.ExecAsync<char>("SELECT @Data", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await DbHelper.ExecAsync<string>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<string>("SELECT @Data", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await DbHelper.ExecAsync<string>("SELECT @Data", value));
        Assert.Equal(value, await DbHelper.ExecAsync<string>("SELECT @Data", sqlValue));

        Assert.Equal(UserType.User, await DbHelper.ExecAsync<UserType?>("SELECT @Data", UserType.User));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_Bytes(byte[] bytes) 
        => Assert.Equal(bytes, await DbHelper.ExecAsync<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_SqlBinary(byte[] bytes)
    {
        var sqlValue = new SqlBinary(bytes);
        Assert.Equal(sqlValue.Value, (await DbHelper.ExecAsync<SqlBinary>("SELECT @Data", sqlValue)).Value);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public async Task CanExecAsync_SqlBytes(byte[] bytes)
    {
        var sqlValue = new SqlBytes(bytes);
        Assert.Equal(sqlValue.Value, (await DbHelper.ExecAsync<SqlBytes>("SELECT @Data", sqlValue))!.Value);
    }

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
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public async Task CanExecAsync_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(sqlValue.Value, (await DbHelper.ExecAsync<SqlChars>("SELECT @Data", sqlValue))!.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_String(string str) 
        => Assert.Equal(str, await DbHelper.ExecAsync<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_TextReader(string text)
    {
        using TextReader readerSource = new StringReader(text);
        using var readerResult = await DbHelper.ExecAsync<TextReader>("SELECT @Data", readerSource);

        Assert.NotNull(readerResult);
        Assert.Equal(text, await readerResult.ReadToEndAsync());
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_SqlXml(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        var sqlValue = new SqlXml(new System.Xml.XmlTextReader(new StringReader(value)));

        var sqlActual = await DbHelper.ExecAsync<SqlXml>("SELECT @Data", sqlValue);

        Assert.NotNull(sqlActual);
        Assert.Equal(sqlActual.IsNull, sqlValue.IsNull);
        Assert.Equal(sqlActual.Value, sqlValue.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public async Task CanExecAsync_XmlReader(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        using var readerIn = XmlReader.Create(new StringReader(value));

        using var readerOut = await DbHelper.ExecAsync<XmlReader>("SELECT @Data", new SqlXml(readerIn));

        Assert.NotNull(readerOut);

        readerOut.Read();

        Assert.Equal(value, await readerOut.ReadOuterXmlAsync());
    }

    [Fact]
    public async Task CanExecAsync_Record()
    {
        Assert.Equal("John", await DbHelper.ExecAsync<string?>("ProcObj", new { Name = "John" }));

        var rec1 = await DbHelper.ExecAsync<JsonObject>("ProcObj1");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = await DbHelper.ExecAsync<List<JsonObject>>("ProcObj2");
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