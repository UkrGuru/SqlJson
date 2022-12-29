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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string?> GetAsync(string? value, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, cancellationToken);

            return file?.FileContent == null ? null : Encoding.UTF8.GetString(file.FileContent);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Load file from current database
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<DbFile?> GetAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var file = await DbHelper.ExecAsync<DbFile>("WJbFiles_Get", guid, cancellationToken: cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// Save file in the current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filename"></param>
    /// <param name="safe"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = default, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 1024)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            return await file.SetAsync(cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Delete file in the current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DelAsync(string? value, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid)) await DelAsync(guid, cancellationToken);
    }

    /// <summary>
    /// Delete file in the current database
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DelAsync(Guid guid, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);
}