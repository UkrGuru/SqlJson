// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.GlobalTests;

namespace UkrGuru.SqlJson;

public class DbCrudHelperTests
{
    public DbCrudHelperTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }
        DbHelper.ConnectionString = GlobalTests.ConnectionString;
    }

    [Fact]
    public async Task CanCrudAsync()
    {
        var item1 = new { Name = "DbHelperName1" };

        var id = await DbHelper.CreateAsync<int?>("""
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(50))

SELECT SCOPE_IDENTITY()
""", item1);

        Assert.NotNull(id);

        var item2 = await DbHelper.ReadAsync<Region?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbHelperName2";

        await DbHelper.UpdateAsync("""
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
""", item2);

        var item3 = await DbHelper.ReadAsync<Region?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await DbHelper.DeleteAsync("""
DELETE TestItems
WHERE Id = @Data
""", id);

        var item4 = await DbHelper.ReadAsync<Region?>("""
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
""", id);

        Assert.Null(item4);
    }


}