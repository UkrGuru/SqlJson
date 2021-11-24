using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace UkrGuru.SqlJson.Tests
{
    public class SqlJsonTest
    {
        public class Region
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public SqlJsonTest()
        {
            var dbName = "SqlJsonTest";
            var connectionString = $"Server=(localdb)\\mssqllocaldb;Database={dbName};Trusted_Connection=True";

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
            Assembly.GetExecutingAssembly().ExecResource("InitDb.sql");

            DbHelper.ConnectionString = connectionString;
            Assembly.GetExecutingAssembly().ExecResource("SeedDb.sql");
        }

        [Fact]
        public void AllTest()
        {
            var regions = DbHelper.FromProc<List<Region>>("Regions_Lst");

            // check all regions
            Assert.Equal(4, regions.Count);
            Assert.Equal("Eastern", regions[0].Name);
            Assert.Equal("Western", regions[1].Name);
            Assert.Equal("Northern", regions[2].Name);
            Assert.Equal("Southern", regions[3].Name);

            // check 1st region
            var region = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
            Assert.Equal("Eastern", region.Name);

            // check no exists region
            region = DbHelper.FromProc<Region>("Regions_Get", new { Id = -1 });
            Assert.Null(region.Name);

            // update region name
            DbHelper.ExecProc("Regions_Upd", new { Id = 1, Name = "Eastern #1" });
            region = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
            Assert.Equal("Eastern #1", region.Name);

            // delete region
            DbHelper.ExecProc("Regions_Del", new { Id = 1 });
            region = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
            Assert.Null(region.Name);
        }
    }
}