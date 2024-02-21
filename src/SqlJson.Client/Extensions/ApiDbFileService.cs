// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Database service for CRUD operations with DbFile object via ApiHole.
/// </summary>
public class ApiDbFileService : ApiDbService, IDbFileService
{
    /// <summary>
    /// Initializes a new instance of the ApiDbFileService class with the specified client.
    /// </summary>
    /// <param name="http">HTTP client instance</param>
    public ApiDbFileService(HttpClient http) : base(http) { }

    /// <inheritdoc/>
    public async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await DeleteAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, timeout, cancellationToken);

            return await file.ToStringAsync();
        }

        return await Task.FromResult(value);
    }

    /// <inheritdoc/>
    public async Task<DbFile?> GetAsync(Guid? guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await ReadAsync<DbFile?>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<Guid?> SetAsync(DbFile? file, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await file.CompressAsync(cancellationToken);

        return await CreateAsync<Guid?>("WJbFiles_Ins", file, timeout, cancellationToken);
    }
}