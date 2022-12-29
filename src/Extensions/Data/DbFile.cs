// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// UkrGuru's files are stored in the Sql Server database.
/// </summary>
public record class DbFile
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public Guid? Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public byte[]? FileContent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool Safe { get; set; } = false;
}
