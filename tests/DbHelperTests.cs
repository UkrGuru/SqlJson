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

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
            Assembly.GetExecutingAssembly().ExecResource("InitDb.sql");

            DbHelper.ConnectionString = connectionString;
            Assembly.GetExecutingAssembly().ExecResource("SeedDb.sql");
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
            Assert.Equal(4, regions.Count);
            Assert.Equal("Eastern", regions[0].Name);
            Assert.Equal("Western", regions[1].Name);
            Assert.Equal("Northern", regions[2].Name);
            Assert.Equal("Southern", regions[3].Name);

            Assert.Equal(1, DbHelper.ExecProc("Regions_Upd", new { Id = 1, Name = "Eastern #1" }));

            var region = DbHelper.FromProc<Region>("Regions_Get", new { Id = 1 });
            Assert.Equal("Eastern #1", region.Name);

            Assert.Equal(1, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
            Assert.Equal(0, DbHelper.ExecProc("Regions_Del", new { Id = 1 }));
        }
    }
}