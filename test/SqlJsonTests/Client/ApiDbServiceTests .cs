// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Client;

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

    public class TestItem
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }

    [Fact]
    public async Task CanCrudAsync()
    {
        var item1 = new { Name = "ApiName1" };

        var id = await _db.CreateAsync<int?>("TestItems_Ins", item1);

        Assert.NotNull(id);

        var item2 = await _db.ReadAsync<TestItem?>("TestItems_Get", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "ApiName2";

        await _db.UpdateAsync("TestItems_Upd", item2);

        var item3 = await _db.ReadAsync<TestItem?>("TestItems_Get", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await _db.DeleteAsync("TestItems_Del", id);

        var item4 = await _db.ReadAsync<TestItem?>("TestItems_Get", id);

        Assert.Null(item4);
    }
}