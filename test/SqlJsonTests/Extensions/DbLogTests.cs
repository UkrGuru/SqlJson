// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions.Tests;

public class DbLogTests
{
    private readonly IDbLogService _dbLog;
    private readonly IDbService _db;

    public DbLogTests()
    {
        DbHelper.ConnectionString = ConnectionString;
        _dbLog = new DbLogService(Configuration);
        _db = new DbService(Configuration);
    }

    [Fact]
    public async Task CanDbLogAsync()
    {
        await DbHelper.DeleteAsync("DelErr", "%DbLogHelper%");

        await DbLogHelper.LogInformationAsync("Information DbLogHelper Async #1", "Information More Async #1");
        await DbLogHelper.LogDebugAsync("Debug DbLogHelper Async #2", "Debug More Async #2");
        await DbLogHelper.LogErrorAsync("Error DbLogHelper Async #3", new { id = 3 });
        Assert.Equal(2, await _db.ReadAsync<int?>("CalcErr_Api", new { Title = "DbLogHelper" }));
        Assert.Equal(1, await _db.ReadAsync<int?>("CalcErr_Api", new { Title = "DbLogHelper", LogLevel = 4 }));

        await DbHelper.DeleteAsync("DelErr", "%DbLogService%");

        await _dbLog.LogInformationAsync("Information DbLogService Async #4", "Information More Async #4");
        await _dbLog.LogDebugAsync("Debug DbLogService Async #5", "Debug More Async");
        await _dbLog.LogErrorAsync("Error DbLogService Async #6", new { id = 6 });
        Assert.Equal(2, await _db.ReadAsync<int?>("CalcErr_Api", new { Title = "DbLogService" }));
        Assert.Equal(1, await _db.ReadAsync<int?>("CalcErr_Api", new { Title = "DbLogService", LogLevel = 4 }));
    }
}