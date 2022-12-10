// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using SqlJsonTests;
using System.Text.Json;
using Xunit;

namespace UkrGuru.SqlJson;

public class DbHelperTests
{
    public DbHelperTests() => DbHelper.ConnectionString = Globals.ConnectionString;

    //[Fact]
    //public static void CreateSqlConnectionTests()
    //{
    //    var connection = DbHelper.CreateSqlConnection();

    //    Assert.NotNull(connection);
    //    Assert.Equal(DbName, connection.Database);
    //    Assert.Equal(ConnectionString, connection.ConnectionString);
    //}

    [Fact]
    public void CanNormalizeParams()
    {
        object data = "str1";
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = true;
        Assert.Equal(data, DbHelper.NormalizeParams(data));
        data = false;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (byte)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));
        data = new byte[] { 1, 2 };
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new char[] { '1', '2' };
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new DateTime(2000, 1, 1);
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new DateTimeOffset(new DateTime(2000, 1, 1));
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (decimal)123.45;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (double)123.45;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = Guid.NewGuid();
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (Int16)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = (Int32)1;
        Assert.Equal(data, DbHelper.NormalizeParams(data));

        data = new { Name = "Proc1" };
        Assert.Equal(JsonSerializer.Serialize(data), DbHelper.NormalizeParams(data));

        data = JsonSerializer.Serialize(new { Name = "Proc1" });
        Assert.Equal(data, DbHelper.NormalizeParams(JsonSerializer.Deserialize<dynamic?>(Convert.ToString(data)!)));
    }

    [Fact]
    public void CanExecCommand()
    {
        var num = DbHelper.ExecCommand("SELECT 1");
        Assert.Equal(-1, num);

        _ = DbHelper.ExecCommand("IF @Data = 'Data' SELECT 'OK' ELSE THROW 51000, 'Invalid @Data as String parameter.', 1;", "Data");

        _ = DbHelper.ExecCommand("IF JSON_VALUE(@Data, '$.Name') = 'John'  SELECT 'OK' ELSE THROW 51000, 'Invalid Name in @Data parameter.', 1;", new { Name = "John" });
    }

    [Fact]
    public async Task CanExecCommandAsync()
    {
        var num = await DbHelper.ExecCommandAsync("SELECT 1");
        Assert.Equal(-1, num);

        _ = DbHelper.ExecCommandAsync("IF @Data = 'Data' SELECT 'OK' ELSE THROW 51000, 'Invalid @Data as String parameter.', 1;", "Data");

        _ = DbHelper.ExecCommandAsync("IF JSON_VALUE(@Data, '$.Name') = 'John'  SELECT 'OK' ELSE THROW 51000, 'Invalid Name in @Data parameter.', 1;", new { Name = "John" });
    }

    [Fact]
    public void CanExecProc()
    {
        _ = DbHelper.ExecCommand("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Table]') AND type in (N'U')) DROP TABLE [dbo].[Table]");

        _ = DbHelper.ExecCommand("CREATE TABLE [dbo].[Table] ([value][sql_variant] NULL) ON [PRIMARY]");

        _ = DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[ExecProcTest] @Data sql_variant = NULL AS INSERT INTO [Table] (Value) VALUES(@Data)");

        _ = DbHelper.ExecProc("ExecProcTest");

        string? null1 = null;
        _ = DbHelper.ExecProc("ExecProcTest", null1);

        var str1 = "String";
        _ = DbHelper.ExecProc("ExecProcTest", str1);

        var class1 = new { Id = 1, Name = "Region1" };
        _ = DbHelper.ExecProc("ExecProcTest", class1);

        var int1 = 123;
        _ = DbHelper.ExecProc("ExecProcTest", int1);

        var double1 = 123.45;
        _ = DbHelper.ExecProc("ExecProcTest", double1);

        var guid1 = Guid.NewGuid();
        _ = DbHelper.ExecProc("ExecProcTest", guid1);

        var dt1 = new DateTime(2000, 1, 1);
        _ = DbHelper.ExecProc("ExecProcTest", dt1);
    }

    [Fact]
    public async Task CanExecProcAsync()
    {
        _ = await DbHelper.ExecCommandAsync("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableAsync]') AND type in (N'U')) DROP TABLE [dbo].[TableAsync]");

        _ = await DbHelper.ExecCommandAsync("CREATE TABLE [dbo].[TableAsync] ([value][sql_variant] NULL) ON [PRIMARY]");

        _ = await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[ExecProcAsyncTest] @Data sql_variant = NULL AS INSERT INTO [TableAsync] (Value) VALUES(@Data)");

        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest");

        string? null1 = null;
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", null1);

        var str1 = "String";
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", str1);

        var class1 = new { Id = 1, Name = "Region1" };
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", class1);

        var int1 = 123;
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", int1);

        var double1 = 123.45;
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", double1);

        var guid1 = Guid.NewGuid();
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", guid1);

        var dt1 = new DateTime(2000, 1, 1);
        _ = await DbHelper.ExecProcAsync("ExecProcAsyncTest", dt1);

    }

    [Fact]
    public void CanFromCommand()
    {
        var num1 = DbHelper.FromCommand<int?>("SELECT CAST(1 as varchar)");
        Assert.Equal(1, num1);

        var num2 = DbHelper.FromCommand<int?>("SELECT CAST(NULL as varchar)");
        Assert.Null(num2);

        var data = DbHelper.FromCommand<string?>("SELECT @Data", "Data");
        Assert.Equal("Data", data);

        var name = DbHelper.FromCommand<string?>("SELECT JSON_VALUE(@Data, '$.Name')", new { Name = "John" });
        Assert.Equal("John", name);
    }

    [Fact]
    public async Task CanFromCommandAsync()
    {
        var num1 = await DbHelper.FromCommandAsync<int?>("SELECT CAST(1 as varchar)");
        Assert.Equal(1, num1);

        var num2 = await DbHelper.FromCommandAsync<int?>("SELECT CAST(NULL as varchar)");
        Assert.Null(num2);

        var data = await DbHelper.FromCommandAsync<string?>("SELECT @Data", "Data");
        Assert.Equal("Data", data);

        var name = await DbHelper.FromCommandAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name')", new { Name = "John" });
        Assert.Equal("John", name);
    }

    [Fact]
    public void CanFromProc()
    {
        _ = DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[FromProcIntTest] @Data int = NULL AS SELECT CAST(@Data as varchar);");

        _ = DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[FromProcStrTest] @Data varchar(100) = NULL AS SELECT @Data;");

        _ = DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[FromProcObjTest] @Data varchar(100) = NULL AS SELECT JSON_VALUE(@Data, '$.Name');");

        var num1 = DbHelper.FromProc<int?>("FromProcIntTest", 1);
        Assert.Equal(1, num1);

        var num2 = DbHelper.FromProc<int?>("FromProcIntTest", null);
        Assert.Null(num2);

        var data = DbHelper.FromProc<string?>("FromProcStrTest", "Data");
        Assert.Equal("Data", data);

        var name = DbHelper.FromProc<string?>("FromProcObjTest", new { Name = "John" });
        Assert.Equal("John", name);
    }

    [Fact]
    public async Task CanFromProcAsync()
    {
        _ = await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[FromProcIntAsyncTest] @Data int = NULL AS SELECT CAST(@Data as varchar);");

        _ = await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[FromProcStrAsyncTest] @Data varchar(100) = NULL AS SELECT @Data;");

        _ = await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[FromProcObjAsyncTest] @Data varchar(100) = NULL AS SELECT JSON_VALUE(@Data, '$.Name');");

        var num1 = DbHelper.FromProc<int?>("FromProcIntAsyncTest", 1);
        Assert.Equal(1, num1);

        var num2 = await DbHelper.FromProcAsync<int?>("FromProcIntAsyncTest", null);
        Assert.Null(num2);

        var data = await DbHelper.FromProcAsync<string?>("FromProcStrAsyncTest", "Data");
        Assert.Equal("Data", data);

        var name = await DbHelper.FromProcAsync<string?>("FromProcObjAsyncTest", new { Name = "John" });
        Assert.Equal("John", name);
    }
}