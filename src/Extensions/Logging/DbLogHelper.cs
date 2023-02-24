// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Logging;

/// <summary>
/// Helper for logging to the database.
/// </summary>
public class DbLogHelper
{
    /// <summary>
    /// MinDbLogLevel allows to set the minimum allowed logging level.
    /// </summary>
    public static DbLogLevel MinDbLogLevel { get; set; } = DbLogLevel.Information;

    /// <summary>
    /// Normalization of parameters before logging.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <returns></returns>
    public static object Normalize(DbLogLevel logLevel, string title, object? more = null)
        => new { LogLevel = logLevel, Title = title, LogMore = more };

    /// <summary>
    /// Synchronous method that writes a log any of type to the database.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void Log(DbLogLevel logLevel, string title, object? more = null)
    {
        try { if ((byte)logLevel >= (byte)MinDbLogLevel) DbHelper.Exec("WJbLogs_Ins", Normalize(logLevel, title, more)); }
        finally { }
    }

    /// <summary>
    /// Synchronous method that writes a critical log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogCritical(string title, object? more = null) => Log(DbLogLevel.Critical, title, more);

    /// <summary>
    /// Synchronous method that writes a debug log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogDebug(string title, object? more = null) => Log(DbLogLevel.Debug, title, more);

    /// <summary>
    /// Synchronous method that writes an error log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogError(string title, object? more = null) => Log(DbLogLevel.Error, title, more);

    /// <summary>
    /// Synchronous method that writes an information log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogInformation(string title, object? more = null) => Log(DbLogLevel.Information, title, more);

    /// <summary>
    /// Synchronous method that writes a trace log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogTrace(string title, object? more = null) => Log(DbLogLevel.Trace, title, more);

    /// <summary>
    /// Synchronous method that writes a warning log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogWarning(string title, object? more = null) => Log(DbLogLevel.Warning, title, more);

    /// <summary>
    /// Asynchronous method that writes a log any of type to the database.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        try { if ((byte)logLevel >= (byte)MinDbLogLevel) await DbHelper.ExecAsync("WJbLogs_Ins", Normalize(logLevel, title, more), cancellationToken: cancellationToken); }
        finally { await Task.CompletedTask; }
    }

    /// <summary>
    /// Asynchronous method that writes a critical log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a debug log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an error log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an information log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a trace log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a warning log to the database.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Warning, title, more, cancellationToken);
}
