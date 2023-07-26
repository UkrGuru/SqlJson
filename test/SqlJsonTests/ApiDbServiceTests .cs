// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json.Nodes;
using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class ApiDbServiceTests
{
    private readonly HttpClient _http;
    private readonly IDbService _db;

    public ApiDbServiceTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        _http = new HttpClient() { BaseAddress = new Uri("https://localhost:7285/") };

        _db = new ApiDbService(_http);
    }

    [Fact]
    public async Task CanUseParamsAsync()
    {
        await _db.DeleteAsync("ProcNull");

        await _db.DeleteAsync("ProcInt", 1);

        await _db.DeleteAsync("ProcInt", null);

        await _db.DeleteAsync("ProcStr", "Data");

        await _db.DeleteAsync("ProcObj", new { Name = "John" });
    }

    [Fact]
    public async Task CanHaveResultsAsync()
    {
        Assert.Null(await _db.ExecAsync<int?>("ProcNull"));

        Assert.Null(await _db.ExecAsync<int?>("ProcInt", null));
        Assert.Equal(1, await _db.ExecAsync<int?>("ProcInt", 1));

        Assert.Equal("Data", await _db.ExecAsync<string?>("ProcStr", "Data"));

        Assert.Equal("John", await _db.ExecAsync<string?>("ProcObj", new { Name = "John" }));
        Assert.Null(await _db.ExecAsync<int?>("ProcInt"));

        //Assert.Equal(true, await _db.ExecAsync<bool?>("SELECT @Data", true));

        //Assert.Equal(Guid.Empty, await _db.ExecAsync<Guid?>("SELECT @Data", Guid.Empty));

        //Assert.Equal('X', await _db.ExecAsync<char?>("SELECT @Data", 'X'));

        //Assert.Equal((byte)1, await _db.ExecAsync<byte?>("SELECT @Data", (byte)1));
        //Assert.Equal(1, await _db.ExecAsync<int?>("SELECT @Data", 1));
        //Assert.Equal((long)1, await _db.ExecAsync<long?>("SELECT @Data", (long)1));
        //Assert.Equal(1.0f, await _db.ExecAsync<float?>("SELECT @Data", 1.0f));
        //Assert.Equal(1.0d, await _db.ExecAsync<double?>("SELECT @Data", 1.0d));
        //Assert.Equal(1.0m, await _db.ExecAsync<decimal?>("SELECT @Data", 1.0m));

        //Assert.Equal(new DateOnly(2000, 1, 1), await _db.ExecAsync<DateOnly?>("ProcDate", new DateOnly(2000, 1, 1)));
        //Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1), await _db.ExecAsync<DateTime?>("ProcDate", new DateTime(2000, 1, 1, 1, 1, 1)));
        //Assert.Equal(new DateTimeOffset(new DateTime(2000, 1, 1)), await _db.ExecAsync<DateTimeOffset?>("ProcDate", new DateTimeOffset(new DateTime(2000, 1, 1))));
        //Assert.Equal(new TimeOnly(1, 1, 1), await _db.ExecAsync<TimeOnly?>("ProcDate", new TimeOnly(1, 1, 1)));
        //Assert.Equal(new TimeSpan(1, 1, 1), await _db.ExecAsync<TimeSpan?>("ProcDate", new TimeSpan(1, 1, 1)));

        //Assert.Equal("John", await _db.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name');", new { Name = "John" }));

        //var rec1 = await _db.ExecAsync<JsonObject>("SELECT 1 Id, 'John' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
        //Assert.NotNull(rec1);
        //Assert.Equal(1, (int?)rec1["Id"]);
        //Assert.Equal("John", (string?)rec1["Name"]);

        //var recs = await _db.ExecAsync<List<JsonObject>>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name FOR JSON PATH");
        //Assert.NotNull(recs);
        //Assert.Equal(2, recs.Count);
        //Assert.Equal(1, (int?)recs[0]["Id"]);
        //Assert.Equal("John", (string?)recs[0]["Name"]);
        //Assert.Equal(2, (int?)recs[1]["Id"]);
        //Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    [Fact]
    public async Task CanCrudAsync()
    {
        var item1 = new { Name = "ApiName1" };

        var id = await _db.CreateAsync<int?>("TestItems_Ins", item1);

        Assert.NotNull(id);

        var item2 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "ApiName2";

        await _db.UpdateAsync("TestItems_Upd", item2);

        var item3 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await _db.DeleteAsync("TestItems_Del", id);

        var item4 = await _db.ReadAsync<Region?>("TestItems_Get", id);

        Assert.Null(item4);
    }
}