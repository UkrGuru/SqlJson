// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// UkrGuru's logs are stored in the Sql Server database.
/// </summary>
public class DbLog
{
    /// <summary>
    /// Date/Time of the log
    /// </summary>
    public DateTime? Logged { get; set; }

    /// <summary>
    /// Level of the log 
    /// </summary>
    public DbLogLevel? LogLevel { get; set; }

    /// <summary>
    /// Brief description
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Text message or json in More format
    /// </summary>
    public string? LogMore { get; set; }
}