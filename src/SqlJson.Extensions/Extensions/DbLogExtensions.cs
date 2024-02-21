// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Helper for logging to the database.
/// </summary>
public class DbLogExtensions
{
    /// <summary>
    /// MinDbLogLevel allows to set the minimum allowed logging level.
    /// </summary>
    public static DbLogLevel MinDbLogLevel { get; set; } = DbLogLevel.Information;

    /// <summary>
    /// Normalization of parameters before logging.
    /// </summary>
    /// <param name="logLevel">The level of the log to write</param>
    /// <param name="title">The title of the log</param>
    /// <param name="more">Additional information to include in the log</param>
    /// <returns></returns>
    public static object Normalize(DbLogLevel logLevel, string title, object? more = default)
        => new DbLog() { LogLevel = logLevel, Title = title, LogMore = more };
}