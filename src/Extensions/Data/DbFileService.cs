// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// 
/// </summary>
public interface IDbFileService
{
    /// <summary>
    /// Delete file in the current database
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    Task DelAsync(object? guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Load file from current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Load file from current database
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    Task<DbFile?> GetAsync(Guid guid, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save file in the current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filename"></param>
    /// <param name="safe"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = default, int? timeout = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
public class DbFileService : DbService, IDbFileService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public DbFileService(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await ExecAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <summary>
    /// Load file from current database
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default)
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
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task<DbFile?> GetAsync(Guid guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await ExecAsync<DbFile>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="filename"></param>
    /// <param name="safe"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The async task.</returns>
    public async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 1024)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            await file.CompressAsync(cancellationToken);

            return await ExecAsync<string>("WJbFiles_Ins", file, timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }
}
