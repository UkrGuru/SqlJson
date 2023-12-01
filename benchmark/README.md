# UkrGuru.SqlJson Samples
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)


![UkrGuru.SqlJson Demo](/assets/bench1.png)

```cs
        [Benchmark]
        public void UkrGuru_SqlJson_SqlUpdate() => DbHelper.Exec("UPDATE [Blogs] SET [LastUpdated] = @Data;", DateTime.UtcNow);

        [Benchmark]
        public void EF_PlainSQL_BulkUpdate()
        {
            var date = DateTime.UtcNow;
            using var context = new MyContext();

            context.Database
                .ExecuteSql($"UPDATE [Blogs] SET [LastUpdated] = {date}; --PlainSQLUpdate");
        }

        [Benchmark(Baseline = true)]
        public void EF_ExecuteUpdate_BulkUpdate()
        {
            var date = DateTime.UtcNow;
            using var context = new MyContext();

            context.Blogs.ExecuteUpdate(
                s => s.SetProperty(
                    blog => blog.LastUpdated, date));
        }

        [Benchmark]
        public void UkrGuru_SqlJson_NetUpdate()
        {
            var date = DateTime.UtcNow;
            var blogs = DbHelper.Exec<List<Blog>>("SELECT [BlogId], [LastUpdated] FROM [Blogs] FOR JSON PATH") ?? new();

            DbHelper.Exec("""
                UPDATE [Blogs] SET [LastUpdated] = D.[LastUpdated]
                FROM OPENJSON(@Data) WITH([BlogId] int, [LastUpdated] datetime2(7)) D
                WHERE [Blogs].[BlogId] = D.[BlogId]
                """, blogs.Select(blog => new { blog.BlogId, LastUpdated = date }));
        }

        [Benchmark]
        public void EF_SaveChanges_NoBulkUpdate()
        {
            var date = DateTime.UtcNow;
            using var context = new MyContext();

            var blogs = context.Blogs.ToList();
            foreach (var blog in blogs)
            {
                blog.LastUpdated = date;
            }

            context.SaveChanges();
        }
```
