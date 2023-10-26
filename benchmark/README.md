# UkrGuru.SqlJson Samples
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)


![UkrGuru.SqlJson Demo](/assets/bench1.png)

```cs
[Benchmark]
public void UkrGuru_SqlJson_SqlUpdate() => DbHelper.Exec("UPDATE [Blogs] SET [LastUpdated] = @Data;", DateTime.UtcNow);

[Benchmark]
public void UkrGuru_SqlJson_NetUpdate()
{
    var date = DateTime.UtcNow;

    var blogs = DbHelper.Exec<List<JsonNode>>("SELECT [BlogId], [LastUpdated] FROM [Blogs] FOR JSON PATH") ?? new();

    DbHelper.Exec("""
        UPDATE [Blogs] SET [LastUpdated] = D.[LastUpdated]
        FROM OPENJSON(@Data) WITH([BlogId] int, [LastUpdated] datetime2(7)) D
        WHERE [Blogs].[BlogId] = D.[BlogId]
        """, blogs.Select(blog => { blog["LastUpdated"] = date; return new { blog.BlogId, blog.LastUpdated }; }));
}
```
