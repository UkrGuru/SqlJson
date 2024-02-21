// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// A helper class for working with DbFile objects.
/// </summary>
public static class DbFileHelper
{
    /// <summary>
    /// Saves a file in the current database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result to return</typeparam>
    /// <param name="file">The file to save</param>
    /// <param name="timeout">The command timeout in seconds</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the operation</returns>
    public static async Task<T?> SetAsync<T>(this DbFile? file, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent?.Length > 0)
        {
            await file.CompressAsync(cancellationToken);

            return await DbHelper.CreateAsync<T?>("WJbFiles_Ins", file, timeout, cancellationToken);
        }

        return await Task.FromResult(default(T?));
    }

    /// <inheritdoc/>
    public static async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await DbHelper.DeleteAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <inheritdoc/>
    public static async Task<string?> GetAsync(string? value, int? timeout = default, CancellationToken cancellationToken = default)
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

    /// <inheritdoc/>
    public static async Task<DbFile?> GetAsync(Guid? guid, int? timeout = default, CancellationToken cancellationToken = default)
    {
        var file = await DbHelper.ReadAsync<DbFile?>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <inheritdoc/>
    public static async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 0)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            await file.CompressAsync(cancellationToken);

            return await DbHelper.CreateAsync<string?>("WJbFiles_Ins", file, timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <inheritdoc/>
    public static async Task<Guid?> SetAsync(DbFile? file, int? timeout = default, CancellationToken cancellationToken = default) 
        => await file.SetAsync<Guid?>(timeout, cancellationToken);
}