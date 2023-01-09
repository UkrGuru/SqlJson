// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Logging;

/// <summary>
/// 
/// </summary>
public class DbLogHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static DbLogLevel MinDbLogLevel { get; set; } = DbLogLevel.Information;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <returns></returns>
    public static object Normalize(DbLogLevel logLevel, string title, object? more = null)
        => new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void Log(DbLogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) return;

        DbHelper.Exec("WJbLogs_Ins", Normalize(logLevel, title, more));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogCritical(string title, object? more = null) => Log(DbLogLevel.Critical, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogDebug(string title, object? more = null) => Log(DbLogLevel.Debug, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogError(string title, object? more = null) => Log(DbLogLevel.Error, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogInformation(string title, object? more = null) => Log(DbLogLevel.Information, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogTrace(string title, object? more = null) => Log(DbLogLevel.Trace, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogWarning(string title, object? more = null) => Log(DbLogLevel.Warning, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) { await Task.CompletedTask; return; }

        await DbHelper.ExecAsync("WJbLogs_Ins", Normalize(logLevel, title, more), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Warning, title, more, cancellationToken);

}
