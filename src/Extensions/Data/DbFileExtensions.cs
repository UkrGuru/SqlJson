// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// Provides extension methods for the DbFile class.
/// </summary>
public static class DbFileExtensions
{
    /// <summary>
    /// Saves a file in the current database asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the result to return</typeparam>
    /// <param name="file">The file to save</param>
    /// <param name="timeout">The command timeout in seconds</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the operation</returns>
    public static async Task<T?> SetAsync<T>(this DbFile file, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return await Task.FromResult<T?>(default);

        // await file.CompressAsync(cancellationToken);

        return await DbHelper.ExecAsync<T?>("WJbFiles_Ins", file, timeout, cancellationToken);
    }

    /// <summary>
    /// Compression of the file content
    /// </summary>
    /// <param name="file">The file to compress</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task CompressAsync(this DbFile? file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return;

        switch (Path.GetExtension(file.FileName ?? "file.txt").ToLower())
        {
            case ".bin":
            case ".bmp":
            case ".csv":
            case ".json":
            case ".htm":
            case ".html":
            case ".txt":
            case ".xml":
                using (var memoryStream = new MemoryStream())
                {
                    using (var compressStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                    {
                        await compressStream.WriteAsync(file.FileContent.AsMemory(0, file.FileContent.Length), cancellationToken);
                    }

                    file.FileContent = memoryStream.ToArray();
                    file.FileName = $"{file.FileName ?? "file.txt"}.gzip";
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Decompression of the file content
    /// </summary>
    /// <param name="file">The file to decompress</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task DecompressAsync(this DbFile? file, CancellationToken cancellationToken = default)
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