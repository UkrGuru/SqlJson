// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO.Compression;
using System.Text;

namespace UkrGuru.SqlJson.Extensions;

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
    public static async Task<T?> SetAsync<T>(this DbFile? file, int? timeout = default, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent?.Length > 0)
        {
            await file.CompressAsync(cancellationToken);

            return await DbHelper.CreateAsync<T?>(DbFileHelper.WJbFiles_Ins, file, timeout, cancellationToken);
        }

        return await Task.FromResult(default(T?));
    }

    /// <summary>
    /// Compression of the file content
    /// </summary>
    /// <param name="file">The file to compress</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task CompressAsync(this DbFile? file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent?.Length > 0)
        {
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

        await Task.CompletedTask;
    }

    /// <summary>
    /// Decompression of the file content
    /// </summary>
    /// <param name="file">The file to decompress</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public static async Task DecompressAsync(this DbFile? file, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent?.Length > 0 && file.FileName?.EndsWith(".gzip") == true)
        {
            using var memoryStream = new MemoryStream(file.FileContent);

            using var outputStream = new MemoryStream();

            using (var decompressStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                await decompressStream.CopyToAsync(outputStream, cancellationToken);
            }

            file.FileContent = outputStream.ToArray();
            file.FileName = file.FileName[..^5];
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// This method returns the file content as a string.
    /// </summary>
    /// <param name="file">The file to convert to string.</param>
    /// <returns>The file content as a string.</returns>
    public static async Task<string?> ToStringAsync(this DbFile? file) => await Task.FromResult(file?.FileContent switch
    {
        null => default,
        _ => Encoding.Unicode.GetString(file.FileContent)
    });
}
