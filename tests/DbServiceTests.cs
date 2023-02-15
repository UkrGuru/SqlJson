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
}
