// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System.Text;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Represents a service for interacting with a database file that inherits from the DbService class and implements the IDbFileService interface.
/// </summary>
public class DbFileService : DbService, IDbFileService
{
    /// <summary>
    /// Initializes a new instance of the DbFileService class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration to use for the service.</param>
    public DbFileService(IConfiguration configuration) : base(configuration) { }

    /// <inheritdoc/>
    public async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await DeleteAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public async Task<string?> GetAsync(string? value, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, timeout, cancellationToken);

            return file?.FileContent != null ? Encoding.UTF8.GetString(file.FileContent) : await Task.FromResult(default(string?));
        }

        return await Task.FromResult(value);
    }

    /// <inheritdoc/>
    public async Task<DbFile?> GetAsync(Guid? guid, int? timeout = default, CancellationToken cancellationToken = default)
    {
        var file = await ReadAsync<DbFile?>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <inheritdoc/>
    public async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = default, CancellationToken cancellationToken = default)
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
    public async Task<Guid?> SetAsync(DbFile? file, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await file.CompressAsync(cancellationToken);

        return await CreateAsync<Guid?>("WJbFiles_Ins", file, timeout, cancellationToken);
    }
}