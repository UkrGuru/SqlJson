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
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogCritical(string title, object? more = null) => Log(DbLogLevel.Critical, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogDebug(string title, object? more = null) => Log(DbLogLevel.Debug, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogError(string title, object? more = null) => Log(DbLogLevel.Error, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogInformation(string title, object? more = null) => Log(DbLogLevel.Information, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogTrace(string title, object? more = null) => Log(DbLogLevel.Trace, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public void LogWarning(string title, object? more = null) => Log(DbLogLevel.Warning, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Warning, title, more, cancellationToken);
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