// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// The base interface of a database service for CRUD operations with DbFile object.
/// </summary>
public interface IDbFileService
{
    /// <summary>
    /// Asynchronously deletes a file from the database by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the file to delete.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DelAsync(object? guid, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a file from the database by its GUID or returns the input value if it is not a valid GUID.
    /// </summary>
    /// <param name="value">The value to parse as a GUID and use to retrieve the file.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the retrieved file as a string, or null if no file was found with the specified GUID or if the input value is not a valid GUID.</returns>
    Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves a file from the database by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the file to retrieve.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved DbFile object, or null if no file was found with the specified GUID.</returns>
    Task<DbFile?> GetAsync(Guid? guid, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously create a file in the database.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="filename">The name of the file to set the value for. Defaults to "file.txt".</param>
    /// <param name="safe">Whether to use safe mode when setting the value. Defaults to false.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">The cancellation token. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value that was set.</returns>
    Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously create a file in the database.
    /// </summary>
    /// <param name="file">The file to be saved.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    Task<Guid?> SetAsync(DbFile? file, int? timeout = null, CancellationToken cancellationToken = default);
}