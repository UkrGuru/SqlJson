// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Helper for logging to the database.
/// </summary>
public class DbLogHelper
{
    /// <summary>
    /// Synchronous method that writes a log any of type to the database.
    /// </summary>
    /// <param name="logLevel">The level of the log to write</param>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    public static void Log(DbLogLevel logLevel, string title, object? more = default)
    {
        try
        {
            if ((byte)logLevel >= (byte)DbLogExtensions.MinDbLogLevel)
            {
                DbHelper.Exec("WJbLogs_Ins", DbLogExtensions.Normalize(logLevel, title, more));
            }
        }
        finally { }
    }

    /// <summary>
    /// Asynchronous method that writes a log any of type to the database.
    /// </summary>
    /// <param name="logLevel">The level of the log to write</param>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogAsync(DbLogLevel logLevel, string title, object? more = default, CancellationToken cancellationToken = default)
    {
        try
        {
            if ((byte)logLevel >= (byte)DbLogExtensions.MinDbLogLevel)
            {
                _ = await DbHelper.ExecAsync("WJbLogs_Ins", DbLogExtensions.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
            }
        }
        finally { await Task.CompletedTask; }
    }

    /// <summary>
    /// Asynchronous method that writes a critical log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogCriticalAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a debug log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogDebugAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an error log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogErrorAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes an information log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogInformationAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a trace log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogTraceAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// Asynchronous method that writes a warning log to the database.
    /// </summary>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task LogWarningAsync(string title, object? more = default, CancellationToken cancellationToken = default)
        => await LogAsync(DbLogLevel.Warning, title, more, cancellationToken);
}