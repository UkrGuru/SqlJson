using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using UkrGuru.SqlJson;

[MemoryDiagnoser]
public class CompiledQueries
{
    private static readonly Func<BloggingContext, IAsyncEnumerable<Blog>> _compiledQuery
        = EF.CompileAsyncQuery((BloggingContext context) => context.Blogs.Where(b => b.Url.StartsWith("http://")));

    private BloggingContext _context;

    [Params(1, 10)]
    public int NumBlogs { get; set; }

    [GlobalSetup]
    public async Task Setup()
    {
        using var context = new BloggingContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await context.SeedDataAsync(NumBlogs);

        _context = new BloggingContext();

        DbHelper.ConnectionString = context.Database.GetConnectionString();
    }

    [Benchmark]
    public async ValueTask<int> WithCompiledQuery()
    {
        var idSum = 0;

        await foreach (var blog in _compiledQuery(_context))
        {
            idSum += blog.Id;
        }

        return idSum;
    }

    [Benchmark]
    public async ValueTask<int> WithoutCompiledQuery()
    {
        var idSum = 0;

        await foreach (var blog in _context.Blogs.Where(b => b.Url.StartsWith("http://")).AsAsyncEnumerable())
        {
            idSum += blog.Id;
        }

        return idSum;
    }

    [Benchmark]
    public async ValueTask<int> WithSqlJson()
        => await DbHelper.ExecAsync<int>("SELECT CAST(SUM(Id) AS varchar(10)) FROM Blogs WHERE Url LIKE '%http://'");

    [GlobalCleanup]
    public ValueTask Cleanup() => _context.DisposeAsync();

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        public async Task SeedDataAsync(int numBlogs)
        {
            Blogs.AddRange(Enumerable.Range(0, numBlogs).Select(i => new Blog { Url = $"http://www.someblog{i}.com"}));
            await SaveChangesAsync();
        }
    }

    public class Blog
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
