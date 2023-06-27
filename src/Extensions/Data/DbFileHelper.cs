// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// A helper class for working with DbFile objects.
/// </summary>
public class DbFileHelper
{
    /// <summary>
    /// Loads a file from the current database asynchronously.
    /// </summary>
    /// <param name="value">The value to parse as a Guid to identify the file.</param>
    /// <param name="timeout">An optional timeout for the operation.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation, containing the file content as a UTF-8 encoded string, or null if the file could not be found.</returns>
    public static async Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, timeout, cancellationToken);

            if (file?.FileContent != null)
                return Encoding.UTF8.GetString(file.FileContent);

            return null;
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Loads a DbFile from the current database asynchronously.
    /// </summary>
    /// <param name="guid">The Guid identifying the file to load.</param>
    /// <param name="timeout">An optional timeout for the operation.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation, containing the loaded DbFile, or null if the file could not be found.</returns>
    public static async Task<DbFile?> GetAsync(Guid? guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await DbHelper.ExecAsync<DbFile>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// Saves a file in the current database asynchronously.
    /// </summary>
    /// <param name="value">The content of the file to save, as a UTF-8 encoded string.</param>
    /// <param name="filename">The name of the file to save.</param>
    /// <param name="safe">A flag indicating whether the file should be saved safely.</param>
    /// <param name="timeout">An optional timeout for the operation.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation, containing the saved file's identifier as a string, or null if the file could not be saved.</returns>
    public static async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = default, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 0)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            return await file.SetAsync<string?>(timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Delete file in the current database
    /// </summary>
    /// <param name="value">The content of the file to save, as a UTF-8 encoded string.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public static async Task DelAsync(object? value, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync("WJbFiles_Del", value, cancellationToken: cancellationToken);
}