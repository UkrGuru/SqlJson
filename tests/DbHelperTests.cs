using System.Collections.Generic;
using System.Reflection;
using Xunit;
using System;

namespace UkrGuru.SqlJson.Tests
{
    public class DbHelperTests
    {
        public class Region
        {
            public int Id { get; set; }
            public string Name { get; set; }
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

        [Fact]
        public void Parameters_Exceptions()
        {
            Assert.Equal("cmdText", Assert.Throws<ArgumentNullException>(() => DbHelper.ExecCommand(null)).ParamName);
            Assert.Equal("cmdText", Assert.Throws<ArgumentException>(() => DbHelper.ExecCommand(string.Empty)).ParamName);

            Assert.Equal("name", Assert.Throws<ArgumentNullException>(() => DbHelper.ExecProc(null)).ParamName);
            Assert.Equal("name", Assert.Throws<ArgumentException>(() => DbHelper.ExecProc(string.Empty)).ParamName);

            Assert.Equal("name", Assert.Throws<ArgumentNullException>(() => DbHelper.FromProc(null)).ParamName);
            Assert.Equal("name", Assert.Throws<ArgumentException>(() => DbHelper.FromProc(string.Empty)).ParamName);
        }

        [Fact]
        public void Proc_Tests()
        {
            List<Region> regions;

            regions = DbHelper.FromProc<List<Region>>("Regions_Lst");
            Assert.Equal(5, regions.Count);
            Assert.Equal("Eastern", regions[0].Name);
            Assert.Equal("Western", regions[1].Name);
            Assert.Equal("Northern", regions[2].Name);
            Assert.Equal("Southern", regions[3].Name);
            Assert.Null(regions[4].Name);

            Assert.Equal(1, DbHelper.ExecProc("Regions_Upd", new { Id = 1, Name = "Eastern #1" }));

            var region = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
            Assert.Equal("Eastern #1", region.Name);

            var regionName = DbHelper.FromProc("Regions_Get_Name", 5);
            Assert.Null(regionName);

            Assert.Equal(1, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
            Assert.Equal(0, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
        }
    }
}