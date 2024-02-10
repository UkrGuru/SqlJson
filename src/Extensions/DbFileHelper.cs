// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// A helper class for working with DbFile objects.
/// </summary>
public class DbFileHelper
{
    internal const string WJbFiles_Ins = "WJbFiles_Ins";
    internal const string WJbFiles_Get = "WJbFiles_Get";
    internal const string WJbFiles_Del = "WJbFiles_Del";

    /// <inheritdoc/>
    public static async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await DbHelper.DeleteAsync(DbFileHelper.WJbFiles_Del, guid, cancellationToken: cancellationToken);

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
        var file = await DbHelper.ReadAsync<DbFile?>(DbFileHelper.WJbFiles_Get, guid, timeout, cancellationToken);

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

            return await DbHelper.CreateAsync<string?>(DbFileHelper.WJbFiles_Ins, file, timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <inheritdoc/>
    public static async Task<Guid?> SetAsync(DbFile? file, int? timeout = default, CancellationToken cancellationToken = default) 
        => await file.SetAsync<Guid?>(timeout, cancellationToken);
}