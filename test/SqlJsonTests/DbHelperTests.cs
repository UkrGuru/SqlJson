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
    public DbHelperTests()
    {
        DbHelper.ConnectionString = ConnectionString;
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

    [Fact]
    public void CanExec_Null()
    {
        Assert.Equal(-1, DbHelper.Exec("DECLARE @Num0 int = 0"));
        Assert.Equal(1, DbHelper.Exec("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)"));

        Assert.Null(DbHelper.Exec<bool?>("DECLARE @Num0 int = 0"));
        Assert.Null(DbHelper.Exec<bool?>("SELECT NULL"));

        Assert.Null(DbHelper.Exec<bool?>("SELECT @Data", DBNull.Value));

        Assert.Null(DbHelper.Exec<byte?>("SELECT @Data", SqlByte.Null));
        Assert.Null(DbHelper.Exec<short?>("SELECT @Data", SqlInt16.Null));
        Assert.Null(DbHelper.Exec<int?>("SELECT @Data", SqlInt32.Null));
        Assert.Null(DbHelper.Exec<long?>("SELECT @Data", SqlInt64.Null));
        Assert.Null(DbHelper.Exec<float?>("SELECT @Data", SqlSingle.Null));
        Assert.Null(DbHelper.Exec<double?>("SELECT @Data", SqlDouble.Null));
        Assert.Null(DbHelper.Exec<decimal?>("SELECT @Data", SqlDecimal.Null));
        Assert.Null(DbHelper.Exec<decimal?>("SELECT @Data", SqlMoney.Null));

        Assert.Null(DbHelper.Exec<DateTime?>("SELECT @Data", SqlDateTime.Null));

        Assert.Null(DbHelper.Exec<bool?>("SELECT @Data", SqlBoolean.Null));
        Assert.Null(DbHelper.Exec<Guid?>("SELECT @Data", SqlGuid.Null));

        Assert.Null(DbHelper.Exec<byte[]?>("SELECT @Data", SqlBinary.Null));
        Assert.Null(DbHelper.Exec<byte[]?>("SELECT @Data", SqlBytes.Null));

        Assert.Null(DbHelper.Exec<char[]?>("SELECT @Data", SqlChars.Null));
        Assert.Null(DbHelper.Exec<string?>("SELECT @Data", SqlString.Null));

        Assert.Null(DbHelper.Exec<string?>("SELECT @Data", SqlXml.Null));
    }

    [Fact]
    public void CanExec_Boolean()
    {
        Assert.True(DbHelper.Exec<bool>("SELECT @Data", true));
        Assert.True(DbHelper.Exec<bool>("SELECT @Data", SqlBoolean.True));

        Assert.False(DbHelper.Exec<bool>("SELECT @Data", false));
        Assert.False(DbHelper.Exec<bool>("SELECT @Data", SqlBoolean.False));
    }

    [Fact]
    public void CanExec_Numeric()
    {
        object? value = default, sqlValue = default;

        value = byte.MinValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", sqlValue));

        value = byte.MaxValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", sqlValue));

        value = short.MaxValue; sqlValue = new SqlInt16((short)value);
        Assert.Equal(value, DbHelper.Exec<short>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<short>("SELECT @Data", sqlValue));

        value = int.MaxValue; sqlValue = new SqlInt32((int)value);
        Assert.Equal(value, DbHelper.Exec<int>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<int>("SELECT @Data", sqlValue));

        value = long.MaxValue; sqlValue = new SqlInt64((long)value);
        Assert.Equal(value, DbHelper.Exec<long>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<long>("SELECT @Data", sqlValue));

        value = decimal.MaxValue; sqlValue = new SqlDecimal((decimal)value);
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", sqlValue));

        value = float.MaxValue; sqlValue = new SqlSingle((float)value);
        Assert.Equal(value, DbHelper.Exec<float>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<float>("SELECT @Data", sqlValue));

        value = double.MaxValue; sqlValue = new SqlDouble((double)value);
        Assert.Equal(value, DbHelper.Exec<double>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<double>("SELECT @Data", sqlValue));

        value = 45m; sqlValue = new SqlMoney((decimal)value);
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", sqlValue));
    }

    [Fact]
    public void CanExec_DateTime()
    {
        object? value = default, sqlValue = default;

        value = DateOnly.MaxValue; 
        Assert.Equal(value, DbHelper.Exec<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, DbHelper.Exec<DateTime>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<DateTime>("SELECT @Data", sqlValue));

        value = new DateTimeOffset((DateTime)value); 
        Assert.Equal(value, DbHelper.Exec<DateTimeOffset>("SELECT @Data", value));
        
        value = new TimeOnly(23, 59, 59); 
        Assert.Equal(value, DbHelper.Exec<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999, 999);
        Assert.Equal(value, DbHelper.Exec<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public void CanExec_Other()
    {
        object? value = default, sqlValue = default;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, DbHelper.Exec<Guid>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<Guid>("SELECT @Data", sqlValue));

        value = 'V'; 
        Assert.Equal(value, DbHelper.Exec<char>("SELECT @Data", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", sqlValue));

        value = UserType.User;
        Assert.Equal(value, DbHelper.Exec<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_Bytes(byte[] bytes) 
        => Assert.Equal(bytes, DbHelper.Exec<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_SqlBinary(byte[] bytes)
    {
        var sqlValue = new SqlBinary(bytes);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlBinary>("SELECT @Data", sqlValue)).Value);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_SqlBytes(byte[] bytes)
    {
        var sqlValue = new SqlBytes(bytes);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlBytes>("SELECT @Data", sqlValue))!.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes), parameters: 4)]
    public void CanExec_Stream(byte[] bytes)
    {
        using var msIn = new MemoryStream(bytes);
        using var stream = DbHelper.Exec<Stream>("SELECT @Data", msIn);

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
    public void CanExec_Chars(char[] chars) 
        => Assert.Equal(chars, DbHelper.Exec<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestChars), parameters: 4)]
    public void CanExec_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlChars>("SELECT @Data", sqlValue))!.Value);
    }


    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public void CanExec_String(string str) 
        => Assert.Equal(str, DbHelper.Exec<string?>("SELECT @Data", str));

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
    public void CanExec_SqlXml(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        var sqlValue = new SqlXml(new System.Xml.XmlTextReader(new StringReader(value)));

        var sqlActual = DbHelper.Exec<SqlXml>("SELECT @Data", sqlValue);

        Assert.NotNull(sqlActual);
        Assert.Equal(sqlActual.IsNull, sqlValue.IsNull);
        Assert.Equal(sqlActual.Value, sqlValue.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestString), parameters: 4)]
    public void CanExec_XmlReader(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        using var readerIn = XmlReader.Create(new StringReader(value));

        using var readerOut = DbHelper.Exec<XmlReader>("SELECT @Data", new SqlXml(readerIn));

        Assert.NotNull(readerOut);

        readerOut.Read();

        Assert.Equal(value, readerOut.ReadOuterXml());
    }

    [Fact]
    public void CanExec_Record()
    {
        Assert.Equal("John", DbHelper.Exec<string?>("ProcObj", new { Name = "John" }));

        var rec1 = DbHelper.Exec<JsonObject>("ProcObj1");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = DbHelper.Exec<List<JsonObject>>("ProcObj2");
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.Equal(1, (int?)recs[0]["Id"]);
        Assert.Equal("John", (string?)recs[0]["Name"]);
        Assert.Equal(2, (int?)recs[1]["Id"]);
        Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    [Fact]
    public void CanExec_Crud()
    {
        var item1 = new { Name = "DbHelperName1" };

        var id = DbHelper.Exec<decimal?>(@"
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(50))

SELECT SCOPE_IDENTITY()
", item1);

        Assert.NotNull(id);

        var item2 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbHelperName2";

        DbHelper.Exec(@"
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
", item2);

        var item3 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        DbHelper.Exec(@"
DELETE TestItems
WHERE Id = @Data
", id);

        var item4 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.Null(item4);
    }
}