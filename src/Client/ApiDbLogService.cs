// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using UkrGuru.Extensions.Logging;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// Api Log service for logging through ApiHole.
/// </summary>
public class ApiDbLogService : ApiDbService, IDbLogService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly DbLogLevel? _minDbLogLevel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="http"></param>
    /// <param name="configuration"></param>
    public ApiDbLogService(HttpClient http, IConfiguration configuration) : base(http)
        => _minDbLogLevel = configuration.GetValue<DbLogLevel?>(MinDbLogLevelPath);

    /// <summary>
    /// 
    /// </summary>
    public virtual string MinDbLogLevelPath => "Logging:LogLevel:UkrGuru.SqlJson";

    /// <summary>
    /// 
    /// </summary>
    public DbLogLevel MinDbLogLevel => _minDbLogLevel ?? DbLogLevel.Information;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void Log(DbLogLevel logLevel, string title, object? more = null)
    {
        try { if ((byte)logLevel >= (byte)MinDbLogLevel) Exec("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more)); }
        finally { }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        try { if ((byte)logLevel >= (byte)MinDbLogLevel) await ExecAsync("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken); }
        finally { await Task.CompletedTask; }
    }
}