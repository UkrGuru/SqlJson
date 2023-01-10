﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson;

public class DbServiceTests
{
    public DbServiceTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }

    [Fact]
    public void CanExec()
    {
        var db = new DbService(GlobalTests.Configuration);

        var num1 = db.Exec("DECLARE @num1 int; SET @num1 = 1;");
        Assert.Equal(-1, num1);

        var num2 = db.Exec<int?>("SELECT CAST(1 as varchar);");
        Assert.Equal(1, num2);

        var num3 = db.Exec<int?>("SELECT CAST(NULL as varchar);");
        Assert.Null(num3);

        var data = db.Exec<string?>("SELECT @Data;", "Data");
        Assert.Equal("Data", data);

        var name = db.Exec<string?>("SELECT JSON_VALUE(@Data, '$.Name');", new { Name = "John" });
        Assert.Equal("John", name);

        var num4 = db.Exec("ProcNull");
        Assert.Equal(-1, num4);

        var num5 = db.Exec<int?>("ProcInt", 1);
        Assert.Equal(1, num5);

        var num6 = db.Exec<int?>("ProcInt", null);
        Assert.Null(num6);

        var data7 = db.Exec<string?>("ProcStr", "Data");
        Assert.Equal("Data", data7);

        var name8 = db.Exec<string?>("ProcObj", new { Name = "John" });
        Assert.Equal("John", name8);
    }

    [Fact]
    public async Task CanExecAsync()
    {
        var db = new DbService(GlobalTests.Configuration);

        var num1 = await db.ExecAsync("DECLARE @num1 int; SET @num1 = 1;");
        Assert.Equal(-1, num1);

        var num2 = await db.ExecAsync<int?>("SELECT CAST(1 as varchar); ");
        Assert.Equal(1, num2);

        var num3 = await db.ExecAsync<int?>("SELECT CAST(NULL as varchar); ");
        Assert.Null(num3);

        var data = await db.ExecAsync<string?>("SELECT @Data; ", "Data");
        Assert.Equal("Data", data);

        var name = await db.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name'); ", new { Name = "John" });
        Assert.Equal("John", name);

        var num4 = await db.ExecAsync("ProcNull");
        Assert.Equal(-1, num4);

        var num5 = await db.ExecAsync<int?>("ProcInt", 1);
        Assert.Equal(1, num5);

        var num6 = await db.ExecAsync<int?>("ProcInt", null);
        Assert.Null(num6);

        var data2 = await db.ExecAsync<string?>("ProcStr", "Data");
        Assert.Equal("Data", data2);

        var name2 = await db.ExecAsync<string?>("ProcObj", new { Name = "John" });
        Assert.Equal("John", name2);
    }
}
