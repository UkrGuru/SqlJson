// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

public class DbLogTests
{
    public DbLogTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }
    
    [Fact]
    public async Task CanDbLog()
    {
        IDbLogService db = new DbLogService(GlobalTests.Configuration);

        DbHelper.Exec("TRUNCATE TABLE WJbLogs");

        DbLogHelper.LogInformation("Information Title #1", "Information More #1");
        DbLogHelper.LogDebug("Debug Title", "Debug More");
        DbLogHelper.LogInformation("Information Title #2", new { id = 2 });
        Assert.Equal(2, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        db.LogInformation("Information Title #3", "Information More #3");
        db.LogDebug("Debug Title", "Debug More");
        db.LogInformation("Information Title #4", new { id = 4 });
        Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        await DbLogHelper.LogInformationAsync("Information Title Async #5", "Information More Async #5");
        await DbLogHelper.LogDebugAsync("Debug Title Async", "Debug More Async");
        await DbLogHelper.LogInformationAsync("Information Title Async #6", new { id = 6 });
        Assert.Equal(6, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        await db.LogInformationAsync("Information Title Async #7", "Information More Async #7");
        await db.LogDebugAsync("Debug Title Async", "Debug More Async");
        await db.LogInformationAsync("Information Title Async #8", new { id = 8 });
        Assert.Equal(8, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));
    }
}