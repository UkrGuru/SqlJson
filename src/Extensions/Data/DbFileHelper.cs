// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// 
/// </summary>
public class DbFileHelper
{
    /// <summary>
    /// Load file from current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
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
    /// Load file from current database
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task<DbFile?> GetAsync(Guid guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await DbHelper.ExecAsync<DbFile>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// Save file in the current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filename"></param>
    /// <param name="safe"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = default, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 1024)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            return await file.SetAsync(timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Delete file in the current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public static async Task DelAsync(object? value, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync("WJbFiles_Del", value, cancellationToken: cancellationToken);
}