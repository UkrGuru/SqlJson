// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Represents a file stored in a SQL Server database.
/// </summary>
public class DbFile
{
    /// <summary>
    /// Gets or sets the unique identifier of the file.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the file was created.
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Gets or sets the content of the file.
    /// </summary>
    public byte[]? FileContent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file is safe.
    /// </summary>
    public bool Safe { get; set; } = false;
}