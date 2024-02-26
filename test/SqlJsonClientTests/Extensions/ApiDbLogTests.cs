// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.SqlJson.Client.Tests.GlobalTests;

namespace UkrGuru.SqlJson.Extensions;

public class ApiDbLogTests
{
    private readonly IDbLogService _dbLog;
    private readonly IDbService _db;

    public ApiDbLogTests()
    {
        _dbLog = new ApiDbLogService(Http, Configuration);
        _db = new ApiDbService(Http);
    }

    [Fact]
    public async Task CanDbLogMInInformation()
    {
        await _db.DeleteAsync("DelErr", "%ApiDbLog%");

        await _dbLog.LogInformationAsync("Information ApiDbLog Async #7", "Information More Async #7");
        await _dbLog.LogDebugAsync("Debug ApiDbLog Async #8", "Debug More Async #8");
        await _dbLog.LogErrorAsync("Error ApiDbLog Async #9", new { id = 9 });
        Assert.Equal(2, await _db.ReadAsync<int?>("CalcErr", new { Title = "ApiDbLog" }));
        Assert.Equal(1, await _db.ReadAsync<int?>("CalcErr", new { Title = "ApiDbLog", LogLevel = 4 }));
    }
}