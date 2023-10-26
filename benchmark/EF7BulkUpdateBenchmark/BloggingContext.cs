using Microsoft.EntityFrameworkCore;

public class MyContext : DbContext
{
    private int _batchSize;

    public MyContext(int batchSize = 42)
    {
        _batchSize = batchSize;
    }

    public DbSet<Blog> Blogs { get; set; }

    public DbSet<RssBlog> RssBlogs { get; set; }

    public DbSet<PrivateBlog> PrivateBlogs { get; set; }

    public DbSet<PrivateBlogWithMFA> PrivateBlogsWithMFA { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EF7BulkUpdate;Trusted_Connection=True", opt => opt.MaxBatchSize(_batchSize));
    //.EnableSensitiveDataLogging()
    //    .LogTo(
    //        s =>  Console.WriteLine(s), LogLevel.Information);
}

public class Blog
{
    public int BlogId { get; set; }
    public string? Url { get; set; }
    public bool? IsActive { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class RssBlog : Blog
{
    public string RssUrl { get; set; }
}

public class PrivateBlog : Blog
{
    public string Password { get; set; }
}

public class PrivateBlogWithMFA : PrivateBlog
{
    public bool mfaOn { get; set; }
}