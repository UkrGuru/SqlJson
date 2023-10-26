using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using Microsoft.EntityFrameworkCore;
using UkrGuru.SqlJson;

namespace BenchmarkDotNet.Samples
{
    [Config(typeof(Config))]
    [SimpleJob(RuntimeMoniker.Net70)]
    [MemoryDiagnoser]
    [HideColumns(Column.AllocRatio, Column.RatioSD, Column.Median, Column.Gen0, Column.Gen1)]
    public class EF7BulkUpdateBenchmark
    {
        private string SQLConnectionString
            = "Data Source=(localdb)\\mssqllocaldb;Database=EF7BulkUpdate;Integrated Security=sspi;";

        [Params(42, 500, 1000)]
        public int records;

        [GlobalSetup]
        public void GlobalSetup()
        {
            using var context = new MyContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var random = new Random();
            var date = DateTime.UtcNow;

            for (int i = 0; i < records; i++)
            {
                context.Add(new
                    Blog
                {
                    Url = "https://blogurl/ID/" + random.Next(1, 10000).ToString(),
                    LastUpdated = date
                });
            }

            context.SaveChanges();
            
            DbHelper.ConnectionString = SQLConnectionString;
        }

        [Benchmark]
        public void UkrGuru_SqlJson_SqlUpdate() => DbHelper.Exec("UPDATE [Blogs] SET [LastUpdated] = @Data;", DateTime.UtcNow);

        [Benchmark]
        public void UkrGuru_SqlJson_NetUpdate()
        {
            var date = DateTime.UtcNow;

            var blogs = DbHelper.Exec<List<Blog>>("SELECT [BlogId], [LastUpdated] FROM [Blogs] FOR JSON PATH") ?? new();

            DbHelper.Exec("""
                UPDATE [Blogs] SET [LastUpdated] = D.[LastUpdated]
                FROM OPENJSON(@Data) WITH([BlogId] int, [LastUpdated] datetime2(7)) D
                WHERE [Blogs].[BlogId] = D.[BlogId]
                """, blogs.Select(blog => { blog.LastUpdated = date; return new { blog.BlogId, blog.LastUpdated }; }));
        }

        [Benchmark]
        public void EF_PlainSQL_BulkUpdate()
        {
            using var context = new MyContext();

            context.Database
                .ExecuteSql($"UPDATE [Blogs] SET [LastUpdated] = {DateTime.UtcNow}; --PlainSQLUpdate");
        }

        [Benchmark(Baseline = true)]
        public void EF_ExecuteUpdate_BulkUpdate()
        {
            using var context = new MyContext();

            context.Blogs.ExecuteUpdate(
                s => s.SetProperty(
                    blog => blog.LastUpdated, DateTime.UtcNow));
        }

        [Benchmark]
        public void EF_SaveChanges_NoBulkUpdate()
        {
            using var context = new MyContext();
            var date = DateTime.UtcNow;

            var blogs = context.Blogs.ToList();
            foreach (var blog in blogs)
            {
                blog.LastUpdated = date;
            }

            context.SaveChanges();
        }

        private class Config : ManualConfig
        {
            public Config()
            {
                SummaryStyle =
                    SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
            }
        }
    }
}
