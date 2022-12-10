// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using SqlJsonTests;
using System.Reflection;
using UkrGuru.SqlJson;
using Xunit;

namespace UkrGuru.Extensions;

public class WJbLogTests
{
    public WJbLogTests()
    {
        if (Globals.DbOk) return;

        DbHelper.ConnectionString = Globals.ConnectionString;

        Assembly.GetAssembly(typeof(DbHelper)).InitDb();

        Globals.DbOk = true;
    }

    [Fact]
    public async Task WJbLogsAsyncTests()
    {
        await DbHelper.ExecProcAsync("WJbLogs_Ins", new { LogLevel = WJbLog.Level.Information, Title = "Test #1", LogMore = "Test #1" });

        await DbHelper.ExecProcAsync("WJbLogs_Ins", new { LogLevel = WJbLog.Level.Information, Title = "Test #2", LogMore = new { jobId = 2, result = "OK" } });

        Assert.True(true);
    }
}
