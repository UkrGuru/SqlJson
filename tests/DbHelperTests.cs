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

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{assembly.GetName().Name}.Resources.InitDb.sql";
        assembly.ExecResource(resourceName);
    }

    //[Fact]
    //public void CreateSqlConnectionTest()
    //{
    //    Assert.True(false, "This test needs an implementation");
    //}

    [Fact]
    public void SyncFuncs_Tests()
    {
        List<Region>? regions = DbHelper.FromProc<List<Region>>("Regions_Lst");

        Assert.NotNull(regions);
        ArgumentNullException.ThrowIfNull(regions);

        Assert.Equal(5, regions.Count);
        Assert.Equal("Eastern", regions[0].Name);
        Assert.Equal("Western", regions[1].Name);
        Assert.Equal("Northern", regions[2].Name);
        Assert.Equal("Southern", regions[3].Name);
        Assert.Null(regions[4].Name);

        Assert.Equal(1, DbHelper.ExecProc("Regions_Upd", new { Id = 1, Name = "Eastern #1" }));

        var region1 = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
        Assert.Equal("Eastern #1", region1?.Name);

        var region6 = DbHelper.FromProc<Region>("Regions_Get", new { Id = 6 });
        Assert.Null(region6);

        var regionName = DbHelper.FromProc("Regions_Get_Name", 5);
        Assert.Null(regionName);

        Assert.Equal(1, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
        Assert.Equal(0, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
    }

    //[Fact]
    //public void ExecProcTest()
    //{
    //    Assert.True(false, "This test needs an implementation");
    //}

    //[Fact]
    //public async Task ExecProcAsyncTest()
    //{
    //    Assert.True(false, "This test needs an implementation");
    //}

    //[Fact]
    //public void FromProcTest()
    //{
    //    Assert.True(false, "This test needs an implementation");
    //}

    //[Fact]
    //public async Task FromProcAsyncTest()
    //{
    //    Assert.True(false, "This test needs an implementation");
    //}
}