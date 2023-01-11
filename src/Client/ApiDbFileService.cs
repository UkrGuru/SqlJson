// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// 
/// </summary>
public class ApiDbFileService : ApiDbService, IDbFileService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="http"></param>
    public ApiDbFileService(HttpClient http) : base(http) { }

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