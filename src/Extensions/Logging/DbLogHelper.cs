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
    /// <param name="logLevel"></param>
    /// <param name="title"></param>
    /// <param name="more"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task LogAsync(DbLogLevel logLevel, string title, object? more = null, CancellationToken cancellationToken = default)
    {
        if ((byte)logLevel < (byte)MinDbLogLevel) { await Task.CompletedTask; return; }

        await DbHelper.ExecAsync("WJbLogs_Ins", Normalize(logLevel, title, more), cancellationToken: cancellationToken);
    }
}
