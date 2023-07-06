﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions.Data;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// Database service for processing or retrieving DbFile object via ApiHole.
/// </summary>
public class ApiDbFileService : ApiDbService, IDbFileService
{
    /// <summary>
    /// Initializes a new instance of the ApiDbFileService class with the specified client.
    /// </summary>
    /// <param name="http">HTTP client instance</param>
    public ApiDbFileService(HttpClient http) : base(http) { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task DelAsync(object? guid, CancellationToken cancellationToken = default) 
        => await DeleteAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, timeout, cancellationToken);

            if (file?.FileContent != null)
                return Encoding.UTF8.GetString(file.FileContent);

            return await Task.FromResult(default(string?));
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<DbFile?> GetAsync(Guid? guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await ReadAsync<DbFile?>("WJbFiles_Get", guid, timeout, cancellationToken);

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
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 0)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            await file.CompressAsync(cancellationToken);

            return await CreateAsync<string?>("WJbFiles_Ins", file, timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Guid?> SetAsync(DbFile? file, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return await Task.FromResult(default(Guid?));

        await file.CompressAsync(cancellationToken);

        return await CreateAsync<Guid?>("WJbFiles_Ins", file, timeout, cancellationToken);
    }
}