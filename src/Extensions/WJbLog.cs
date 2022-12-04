// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.Extensions;

/// <summary>
/// UkrGuru's logs are stored in the Sql Server database.
/// </summary>
public sealed class WJbLog
{
    /// <summary>
    /// Represents the log level for UrkGuru Logs.
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// Logs that contain the most detailed messages. These messages may contain sensitive application data.
        /// These messages are disabled by default and should never be enabled in a production environment.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Logs that are used for interactive investigation during development.  These logs should primarily contain
        /// information useful for debugging and have no long-term value.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs that track the general flow of the application. These logs should have long-term value.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the
        /// application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure. These should indicate a
        /// failure in the current activity, not an application-wide failure.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or a catastrophic failure that requires
        /// immediate attention.
        /// </summary>
        Critical = 5,

        /// <summary>
        /// Not used for writing log messages. Specifies that a logging category should not write any messages.
        /// </summary>
        None = 6,
    }

    /// <summary>
    /// Date/Time of the log
    /// </summary>
    public DateTime? Logged { get; set; }

    /// <summary>
    /// Level of the log 
    /// </summary>
    public Level? LogLevel { get; set; }

    /// <summary>
    /// Brief description
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Text message or json in More format
    /// </summary>
    public string? LogMore { get; set; }
}