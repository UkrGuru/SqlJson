// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text.Json;
using UkrGuru.SqlJson;
using LogLevel = UkrGuru.Extensions.WJbLog.Level;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public static partial class WJbLogHelper
{
    /// <summary>
    /// 
    /// </summary>
    public static LogLevel MinLogLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// 
    /// </summary>
    public static async Task LogTraceAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Trace, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogDebugAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Debug, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogInformationAsync(string title, object? more = null, CancellationToken cancellationToken = default) =>
        await LogAsync(LogLevel.Information, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogWarningAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Warning, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogErrorAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Error, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogCriticalAsync(string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(LogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    public static async Task LogAsync(LogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinLogLevel) return;

        try
        {
            _ = await DbHelper.ExecProcAsync("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) },
                  cancellationToken: cancellationToken);
        }
        catch { }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LogTrace(string title, object? more = null) => Log(LogLevel.Trace, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogDebug(string title, object? more = null) => Log(LogLevel.Debug, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogInformation(string title, object? more = null) => Log(LogLevel.Information, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogWarning(string title, object? more = null) => Log(LogLevel.Warning, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogError(string title, object? more = null) => Log(LogLevel.Error, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogCritical(string title, object? more = null) => Log(LogLevel.Critical, title, more);

    /// <summary>
    /// 
    /// </summary>
    public static void Log(LogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)MinLogLevel) return;

        try
        {
            DbHelper.ExecProc("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) });
        }
        catch { }
    }

    /// <summary>
    /// 
    /// </summary>
    public static async Task LogTraceAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Trace, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogDebugAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Debug, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogInformationAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Information, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogWarningAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Warning, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogErrorAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Error, title, more, cancellationToken);
    /// <summary>
    /// 
    /// </summary>
    public static async Task LogCriticalAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await LogAsync(connection, LogLevel.Critical, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    public static async Task LogAsync(this SqlConnection connection, LogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)WJbLogHelper.MinLogLevel) return;

        try
        {
            _ = await connection.ExecProcAsync("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) },
                  cancellationToken: cancellationToken);
        }
        catch { }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LogTrace(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Trace, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogDebug(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Debug, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogInformation(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Information, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogWarning(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Warning, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogError(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Error, title, more);
    /// <summary>
    /// 
    /// </summary>
    public static void LogCritical(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Critical, title, more);

    /// <summary>
    /// 
    /// </summary>
    public static void Log(this SqlConnection connection, LogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)WJbLogHelper.MinLogLevel) return;

        try
        {
            connection.ExecProc("WJbLogs_Ins", new { LogLevel = logLevel, Title = title, LogMore = more is string ? more : JsonSerializer.Serialize(more) });
        }
        catch { }
    }
}