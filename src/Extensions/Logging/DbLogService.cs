// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Logging;

/// <summary>
/// 
/// </summary>
public interface IDbLogService
{
    /// <summary>
    /// 
    /// </summary>
    string MinDbLogLevelPath { get; }

    /// <summary>
    /// 
    /// </summary>
    DbLogLevel MinDbLogLevel { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    void Log(DbLogLevel logLevel, string title, object? more = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
public class DbLogService : DbService, IDbLogService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly DbLogLevel? _minDbLogLevel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public DbLogService(IConfiguration configuration) : base(configuration) 
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
        if ((byte)logLevel < (byte)MinDbLogLevel) return;

        Exec("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) { await Task.CompletedTask; return; }

        await ExecAsync("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more), cancellationToken : cancellationToken);
    }
}

/// <summary>
/// 
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
    /// <exception cref="NotImplementedException"></exception>
    public void Log(DbLogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) return;

        Exec("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) { await Task.CompletedTask; return; }

        await ExecAsync("WJbLogs_Ins", DbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
    }
}