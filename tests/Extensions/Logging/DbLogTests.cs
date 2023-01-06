// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

public class DbLogTests
{
    public DbLogTests() { int i = 0; while (!GlobalTests.DbOk && i++ < 100) { Thread.Sleep(100); } }
    
    [Fact]
    public async Task DbLogsAsyncTests()
    {
        int n = await DbHelper.ExecAsync("WJbLogs_Ins", new { LogLevel = DbLogLevel.Information, Title = "Test #1", LogMore = "Test #1" });
        Assert.Equal(1, n);

        n = await DbHelper.ExecAsync("WJbLogs_Ins", new { LogLevel = DbLogLevel.Information, Title = "Test #2", LogMore = new { jobId = 2, result = "OK" } });
        Assert.Equal(1, n);
    }
}
