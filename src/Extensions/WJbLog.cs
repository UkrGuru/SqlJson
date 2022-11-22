// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public partial class WJbLog
{
    /// <summary>
    /// 
    /// </summary>
    public enum Level
    {
        /// <summary>
        /// </summary>
        Trace = 0,

        /// <summary>
        /// </summary>
        Debug = 1,

        /// <summary>
        /// </summary>
        Information = 2,

        /// <summary>
        /// </summary>
        Warning = 3,

        /// <summary>
        /// </summary>
        Error = 4,

        /// <summary>
        /// </summary>
        Critical = 5,

        /// <summary>
        /// </summary>
        None = 6,
    }


    /// <summary>
    /// 
    /// </summary>
    [Key]
    [Display(Name = "Id")]
    public int LogId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? Logged { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Level? LogLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? LogMore { get; set; }
}
