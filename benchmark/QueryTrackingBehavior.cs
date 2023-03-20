using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using UkrGuru.SqlJson;

namespace Benchmarks;

[MemoryDiagnoser]
public class QueryTrackingBehavior
{
    [Params(10)]
    public int NumBlogs { get; set; }

    [Params(20)]
    public int NumPostsPerBlog { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        Console.WriteLine("Setting up database...");
        using var context = new BloggingContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        BloggingContext.SeedData(NumBlogs, NumPostsPerBlog);
        Console.WriteLine("Setup complete.");

        DbHelper.ConnectionString = context.Database.GetConnectionString();
        DbHelper.Exec(@"CREATE OR ALTER PROCEDURE Posts_Grd_With_Blog AS 
SELECT Posts.PostId, Posts.Title, Posts.[Content], Posts.BlogId, 
    Blogs.BlogId AS 'Blog.BlogId', Blogs.Url 'Blog.Url', Blogs.Rating 'Blog.Rating' 
FROM Posts 
LEFT JOIN Blogs ON Posts.BlogId = Blogs.BlogId
FOR JSON PATH
");
    }

    [Benchmark(Baseline = true)]
    public List<Post> AsTracking()
    {
        using var context = new BloggingContext();
        return context.Posts.AsTracking().Include(p => p.Blog).ToList();
    }

    [Benchmark]
    public List<Post> AsNoTracking()
    {
        using var context = new BloggingContext();
        return context.Posts.AsNoTracking().Include(p => p.Blog).ToList();
    }

    [Benchmark]
    public List<Post> AsSqlJson() 
        => DbHelper.Exec<List<Post>>("Posts_Grd_With_Blog");

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");

        public static void SeedData(int numBlogs, int numPostsPerBlog)
        {
            using var context = new BloggingContext();
            context.AddRange(
                Enumerable.Range(0, numBlogs).Select(
                    _ => new Blog { Posts = Enumerable.Range(0, numPostsPerBlog).Select(_ => new Post()).ToList() }));
            context.SaveChanges();
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public int Rating { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}