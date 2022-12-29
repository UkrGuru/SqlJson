// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

public class DbLogTests
{
    public DbLogTests()
    {
        if (GlobalTests.DbOk) return;

        DbHelper.ConnectionString = GlobalTests.ConnectionString;

        Assembly.GetAssembly(typeof(DbHelper)).InitDb();

        GlobalTests.DbOk = true;
    }

    [Fact]
    public async Task DbLogsAsyncTests()
    {
        await DbHelper.ExecAsync("WJbLogs_Ins", new { LogLevel = DbLogLevel.Information, Title = "Test #1", LogMore = "Test #1" });

        await DbHelper.ExecAsync("WJbLogs_Ins", new { LogLevel = DbLogLevel.Information, Title = "Test #2", LogMore = new { jobId = 2, result = "OK" } });

        Assert.True(true);
    }
}
