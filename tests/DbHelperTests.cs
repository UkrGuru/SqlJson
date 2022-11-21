using UkrGuru.Extensions;
using Xunit;

namespace UkrGuru.SqlJson.Tests;

public class DbHelperTests
{
    public class Region
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private readonly bool dbOK = false;

    public DbHelperTests()
    {
        var dbName = "SqlJsonTest";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        DbHelper.ConnectionString = connectionString.Replace(dbName, "master");

        DbHelper.ExecCommand($"IF DB_ID('{dbName}') IS NULL CREATE DATABASE {dbName};");

        DbHelper.ConnectionString = connectionString;

        if ( dbOK ) return;

        dbOK = true;
    }

    [Fact]
    public void ParamDataTests()
    {
        _ = DbHelper.ExecCommand("IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Table]') AND type in (N'U')) DROP TABLE [dbo].[Table]");

        _ = DbHelper.ExecCommand("CREATE TABLE [dbo].[Table] ([value][sql_variant] NULL) ON [PRIMARY]");

        _ = DbHelper.ExecCommand("CREATE OR ALTER PROCEDURE [dbo].[StrTest] @Data sql_variant = null AS INSERT INTO [Table] (Value) VALUES(@Data)");

        _ = DbHelper.ExecProc("StrTest");

        string? null1 = null;
        _ = DbHelper.ExecProc("StrTest", null1);

        var str1 = "String";
        _ = DbHelper.ExecProc("StrTest", str1);

        var class1 = new { Id = 1, Name = "Region1" };
        _ = DbHelper.ExecProc("StrTest", class1);

        var int1 = 123;
        _ = DbHelper.ExecProc("StrTest", int1);

        var double1 = 123.45;
        _ = DbHelper.ExecProc("StrTest", double1);

        var guid1 = Guid.NewGuid();
        _ = DbHelper.ExecProc("StrTest", guid1);

        var dt1 = new DateTime(2000, 1, 1);
        _ = DbHelper.ExecProc("StrTest", dt1);

        Assert.True(true);     
    }

    [Fact]
    public void ToObjTests()
    {
        var b1 = ((string?)null).ToObj<bool?>();
        var b2 = "".ToObj<bool?>();
        var b3 = "true".ToObj<bool>();

        var n0 = ((string?)null).ToObj<int?>();
        var n1 = "123".ToObj<int?>();

        var g0 = ((string?)null).ToObj<Guid?>();
        var g1 = Guid.NewGuid().ToString().ToObj<Guid>();

        var s0 = ((string?)null).ToObj<string?>();
        var s1 = "true".ToObj<string>();

        var d0 = ((string?)null).ToObj<DateTime?>();
        var d1 = "Feb 17 2022 11:58AM".ToObj<DateTime>();

        var r0 = ((string?)null).ToObj<Region?>();
        var r1 = @"{ ""Id"" : 1 }".ToObj<Region>();

        Assert.True(true);
    }

    [Fact]
    public async Task RunSqlProcNullTest()
    {
        await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[NullTest] AS SELECT 'OK'");
        var data = null as string;
        var proc_result = DbHelper.FromProc<string?>("NullTest", data);

        Assert.Equal("OK", proc_result);
    }

    [Fact]
    public async Task RunSqlProcDataTest()
    {
        await DbHelper.ExecCommandAsync("CREATE OR ALTER PROCEDURE [dbo].[DataTest] (@Data varchar(100)) AS SELECT @Data");

        var data = "DATA";
        var proc_result = DbHelper.FromProc<string?>("DataTest", data);

        Assert.Equal(data, proc_result);
    }
}