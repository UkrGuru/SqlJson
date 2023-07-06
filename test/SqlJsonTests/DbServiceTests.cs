// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson;

public class DbServiceTests
{
    public DbServiceTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

    [Fact]
    public void CanExec()
    {
        var db = new DbService(GlobalTests.Configuration);

        Assert.Null(db.Exec<bool?>("SELECT NULL"));
        Assert.True(db.Exec<bool?>("SELECT @Data", true));

        Assert.Equal(-1, db.Exec("ProcNull"));
        Assert.Equal(1, db.Exec<int?>("ProcInt", 1));
    }

    [Fact]
    public async Task CanExecAsync()
    {
        var db = new DbService(GlobalTests.Configuration);

        Assert.Null(await db.ExecAsync<bool?>("SELECT NULL"));
        Assert.True(await db.ExecAsync<bool?>("SELECT @Data", true));

        Assert.Equal(-1, await db.ExecAsync("ProcNull"));
        Assert.Equal(1, await db.ExecAsync<int?>("ProcInt", 1));
    }

    public class TestItem
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }

    [Fact]
    public async Task CanCrudAsync()
    {
        var db = new DbService(GlobalTests.Configuration);

        var item1 = new { Name = "DbName1" };

        var id = await db.CreateAsync<int?>("""
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(50))

SELECT SCOPE_IDENTITY()
""", item1);

        Assert.NotNull(id);

        var item2 = await db.ReadAsync<TestItem?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbName2";

        await db.UpdateAsync("""
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
""", item2);

        var item3 = await db.ReadAsync<TestItem?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await db.DeleteAsync("""
DELETE TestItems
WHERE Id = @Data
""", id);

        var item4 = await db.ReadAsync<TestItem?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.Null(item4);
    }
}