// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using UkrGuru.SqlJson;
using LogLevel = UkrGuru.Extensions.WJbLog.Level;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public static class WJbLogExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <returns></returns>
    public static int Log(this SqlConnection connection, LogLevel logLevel, string title, object? more = null)
    {
        if ((byte)logLevel < (byte)WJbLogHelper.MinLogLevel) return 0;

        return connection.ExecProc("WJbLogs_Ins", WJbLogHelper.Normalize(logLevel, title, more));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogTrace(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Trace, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogDebug(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Debug, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogInformation(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Information, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogWarning(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Warning, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogError(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Error, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    public static void LogCritical(this SqlConnection connection, string title, object? more = null) => connection.Log(LogLevel.Critical, title, more);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> LogAsync(this SqlConnection connection, LogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)WJbLogHelper.MinLogLevel) return await Task.FromResult(0);

        return await connection.ExecProcAsync("WJbLogs_Ins", WJbLogHelper.Normalize(logLevel, title, more), cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogTraceAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Trace, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogDebugAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Debug, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogInformationAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Information, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogWarningAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Warning, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogErrorAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Error, title, more, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogCriticalAsync(this SqlConnection connection, string title, object? more = null, CancellationToken cancellationToken = default)
        => await connection.LogAsync(LogLevel.Critical, title, more, cancellationToken);
}