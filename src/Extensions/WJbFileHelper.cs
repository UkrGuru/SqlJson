// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;
using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public static class WJbFileHelper
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string?> GetAsync(string? value, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, cancellationToken);

            if (file?.FileContent == null) return null;

            return Encoding.UTF8.GetString(file.FileContent);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<WJbFile?> GetAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var file = await DbHelper.FromProcAsync<WJbFile>("WJbFiles_Get", guid, cancellationToken: cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// 
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
            WJbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };

            return await file.SetAsync(cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<string?> SetAsync(this WJbFile file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return await Task.FromResult(null as string);

        switch (Path.GetExtension(file.FileName ?? "file.txt").ToLower())
        {
            case ".bmp":
            case ".csv":
            case ".json":
            case ".htm":
            case ".html":
            case ".txt":
            case ".xml":
                await file.CompressAsync(cancellationToken);
                break;
        }

        return await DbHelper.FromProcAsync<string>("WJbFiles_Ins", file, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DelAsync(string? value, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(value) && Guid.TryParse(value, out Guid guid)) await DelAsync(guid, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DelAsync(Guid guid, CancellationToken cancellationToken = default)
        => await DbHelper.ExecProcAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task CompressAsync(this WJbFile? file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        using var memoryStream = new MemoryStream();

        using (var compressStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
        {
            await compressStream.WriteAsync(file.FileContent.AsMemory(0, file.FileContent.Length), cancellationToken);
        }

        file.FileContent = memoryStream.ToArray();
        file.FileName = $"{file.FileName ?? "file.txt"}.gzip";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task DecompressAsync(this WJbFile? file, CancellationToken cancellationToken = default)
    {
        if (file?.FileName == null || !file.FileName.EndsWith(".gzip")) return;

        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        using var memoryStream = new MemoryStream(file.FileContent);

        using var outputStream = new MemoryStream();

        using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        {
            await decompressStream.CopyToAsync(outputStream, cancellationToken);
        }

        file.FileContent = outputStream.ToArray();
        file.FileName = file.FileName[..^5];
    }
}