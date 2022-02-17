using System.Reflection;
using Xunit;

namespace UkrGuru.SqlJson.Tests;

public class DbHelperTests
{
    public class Region
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public DbHelperTests()
    {
        var dbName = "SqlJsonTest";

        var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

        var dbInitScript = $"IF DB_ID('{dbName}') IS NOT NULL BEGIN " +
$"  ALTER DATABASE {dbName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
$"  DROP DATABASE {dbName}; " +
$"END " +
$"CREATE DATABASE {dbName};";

        DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
        DbHelper.ExecCommand(dbInitScript);

        DbHelper.ConnectionString = connectionString;

        //        var assembly = Assembly.GetExecutingAssembly();
        //        var resourceName = $"{assembly.GetName().Name}.Resources.InitDb.sql";
        //        assembly.ExecResource(resourceName);
    }

    [Fact]
    public void InputData_Tests()
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

        var double1 = 123.123;
        _ = DbHelper.ExecProc("StrTest", double1);

        var guid1 = Guid.NewGuid();
        _ = DbHelper.ExecProc("StrTest", guid1);

        var dt1 = new DateTime(2000, 1, 1);
        _ = DbHelper.ExecProc("StrTest", dt1);

        Assert.True(true);     
    }

    [Fact]
    public void ToObjTest()
    {
        var b1 = ((string?)null).ToObj<bool?>();

        var b2 = "".ToObj<bool?>();

        var b3 = "true".ToObj<bool>();

        var n = "123".ToObj<int>();

        var g1 = Guid.NewGuid().ToString().ToObj<Guid>();

        var s = "true".ToObj<string>();

        var dt = "Feb 17 2022 11:58AM".ToObj<DateTime>();

        var r1 = @"{ ""Id"" : 1 }".ToObj<Region>();

        Assert.True(true);
    }

    //[Fact]
    //public async Task ToObjTestAsync()
    //{
    //    var b1 = await ((string?)null).ToObjAsync<bool?>();

    //    var b2 = await "".ToObjAsync<bool?>();

    //    var b3 = await "true".ToObjAsync<bool>();

    //    var n = await "123".ToObjAsync<int>();

    //    var s = await "true".ToObjAsync<string>();

    //    var r1 = await @"{ ""Id"" : 1 }".ToObjAsync<Region>();

    //    Assert.True(true);
    //}
}