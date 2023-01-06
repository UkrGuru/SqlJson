using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using UkrGuru.SqlJson;
using static System.Runtime.InteropServices.JavaScript.JSType;

[MemoryDiagnoser]
public class DynamicallyConstructedQueries
{
    private int _blogNumber;

    [GlobalSetup]
    public static void GlobalSetup()
    {
        using var context = new BloggingContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        DbHelper.ConnectionString = context.Database.GetConnectionString();
        DbHelper.Exec("CREATE OR ALTER PROCEDURE Blogs_Get_Count @Data nvarchar(1000) AS SELECT CAST(COUNT(BlogId) AS varchar(10)) FROM Blogs WHERE LEN(@Data) > 0 AND Url = @Data");
    }

    #region WithConstant
    [Benchmark]
    public int WithConstant()
    {
        return GetBlogCount("blog" + Interlocked.Increment(ref _blogNumber));

        static int GetBlogCount(string url)
        {
            using var context = new BloggingContext();

            IQueryable<Blog> blogs = context.Blogs;

            if (url is not null)
            {
                var blogParam = Expression.Parameter(typeof(Blog), "b");
                var whereLambda = Expression.Lambda<Func<Blog, bool>>(
                    Expression.Equal(
                        Expression.MakeMemberAccess(
                            blogParam,
                            typeof(Blog).GetMember(nameof(Blog.Url)).Single()
                        ),
                        Expression.Constant(url)),
                    blogParam);

                blogs = blogs.Where(whereLambda);
            }

            return blogs.Count();
        }
    }
    #endregion

    #region WithParameter
    [Benchmark]
    public int WithParameter()
    {
        return GetBlogCount("blog" + Interlocked.Increment(ref _blogNumber));

        int GetBlogCount(string url)
        {
            using var context = new BloggingContext();

            IQueryable<Blog> blogs = context.Blogs;

            if (url is not null)
            {
                blogs = blogs.Where(b => b.Url == url);
            }

            return blogs.Count();
        }
    }
    #endregion

    #region SqlJsonWithParameter
    [Benchmark]
    public int SqlJsonWithParameter()
    {
        return GetBlogCount("blog" + Interlocked.Increment(ref _blogNumber));

        int GetBlogCount(string url) => DbHelper.Exec<int>("Blogs_Get_Count", url);
    }
    #endregion

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True");
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
