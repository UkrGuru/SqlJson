// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Client;

namespace UkrGuru.Extensions;

public class ApiDbLogTests
{
    private readonly HttpClient _http;
    private readonly IDbLogService _dbLog;

    public ApiDbLogTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        _http = new HttpClient() { BaseAddress = new Uri("https://localhost:7285/") };

        _dbLog = new ApiDbLogService(_http, GlobalTests.Configuration);
    }
    
    [Fact]
    public async Task CanDbLog()
    {
        //await _dbLog.  ("TRUNCATE TABLE WJbLogs");

        //DbLogHelper.LogInformation("Information Helper #1", "Information More #1");
        //DbLogHelper.LogDebug("Debug Helper", "Debug More");
        //DbLogHelper.LogError("Error Helper #2", new { id = 2 });
        //Assert.Equal(2, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        //db.LogInformation("Information Service #3", "Information More #3");
        //db.LogDebug("Debug Service", "Debug More");
        //db.LogError("Error Service #4", new { id = 4 });
        //Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        await DbLogHelper.LogInformationAsync("Information Helper Async #5", "Information More Async #5");
        await DbLogHelper.LogDebugAsync("Debug Helper Async", "Debug More Async");
        await DbLogHelper.LogErrorAsync("Error Helper Async #6", new { id = 6 });
        Assert.Equal(6, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        await _dbLog.LogInformationAsync("Information Service Async #7", "Information More Async #7");
        await _dbLog.LogDebugAsync("Debug Service Async", "Debug More Async");
        await _dbLog.LogErrorAsync("Error Service Async #8", new { id = 8 });
        Assert.Equal(8, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs"));

        Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE 'Information%'"));
        Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE 'Error%'"));

        Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE LogLevel = 2"));
        Assert.Equal(4, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE LogLevel = 4"));
    }
}