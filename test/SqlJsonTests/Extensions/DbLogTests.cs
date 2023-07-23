// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Extensions;

public class DbLogTests
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _http;
    private readonly IDbLogService _dbLog;
    private readonly IDbLogService _apiDbLog;

    public DbLogTests()
    {
        int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); }

        DbHelper.ConnectionString = GlobalTests.ConnectionString;

        var inMemorySettings = new Dictionary<string, string?>() {
            { "ConnectionStrings:DefaultConnection", GlobalTests.ConnectionString},
            { "Logging:LogLevel:UkrGuru.SqlJson", "Information" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _http = new HttpClient() { BaseAddress = new Uri("https://localhost:7285/") };

        _dbLog = new DbLogService(_configuration);

        _apiDbLog = new ApiDbLogService(_http, _configuration);
    }

    [Fact]
    public async Task CanDbLogMInInformation()
    {
        DbHelper.Exec("TRUNCATE TABLE WJbLogs");

        await DbLogHelper.LogInformationAsync("Information DbLogHelper Async #1", "Information More Async #1");
        await DbLogHelper.LogDebugAsync("Debug DbLogHelper Async #2", "Debug More Async #2");
        await DbLogHelper.LogErrorAsync("Error DbLogHelper Async #3", new { id = 3 });
        Assert.Equal(2, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% DbLogHelper %'"));
        Assert.Equal(1, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% DbLogHelper %' AND LogLevel = 4"));

        await _dbLog.LogInformationAsync("Information DbLogService Async #4", "Information More Async #4");
        await _dbLog.LogDebugAsync("Debug DbLogService Async #5", "Debug More Async");
        await _dbLog.LogErrorAsync("Error DbLogService Async #6", new { id = 6 });
        Assert.Equal(2, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% DbLogService %'"));
        Assert.Equal(1, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% DbLogService %' AND LogLevel = 4"));

        await _apiDbLog.LogInformationAsync("Information ApiDbLogService Async #7", "Information More Async #7");
        await _apiDbLog.LogDebugAsync("Debug ApiDbLogService Async #8", "Debug More Async #8");
        await _apiDbLog.LogErrorAsync("Error ApiDbLogService Async #9", new { id = 9 });
        Assert.Equal(2, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% ApiDbLogService %'"));
        Assert.Equal(1, DbHelper.Exec<int?>("SELECT COUNT(*) FROM WJbLogs WHERE Title LIKE '% ApiDbLogService %' AND LogLevel = 4"));
    }
}