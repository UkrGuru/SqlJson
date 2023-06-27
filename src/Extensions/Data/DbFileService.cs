// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using System.Text;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions.Data;

/// <summary>
/// Represents a service for interacting with a database file.
/// </summary>
public interface IDbFileService
{
    /// <summary>
    /// Asynchronously opens a database connection, executes a Transact-SQL statement, and returns the number of rows affected.
    /// </summary>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of rows affected.</returns>
    Task<int> ExecAsync(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Result as an object</returns>
    Task<T?> ExecAsync<T>(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes a file from the database by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the file to delete.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DelAsync(object? guid, CancellationToken cancellationToken = default)
        => await ExecAsync("WJbFiles_Del", guid, cancellationToken: cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a file from the database by its GUID or returns the input value if it is not a valid GUID.
    /// </summary>
    /// <param name="value">The value to parse as a GUID and use to retrieve the file.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the contents of the retrieved file as a string, or null if no file was found with the specified GUID or if the input value is not a valid GUID.</returns>
    public async Task<string?> GetAsync(string? value, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(value, out Guid guid))
        {
            var file = await GetAsync(guid, timeout, cancellationToken);

            if (file?.FileContent != null)
                return Encoding.UTF8.GetString(file.FileContent);

            return null;
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Asynchronously retrieves a file from the database by its GUID.
    /// </summary>
    /// <param name="guid">The GUID of the file to retrieve.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved DbFile object, or null if no file was found with the specified GUID.</returns>
    public async Task<DbFile?> GetAsync(Guid? guid, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var file = await ExecAsync<DbFile>("WJbFiles_Get", guid, timeout, cancellationToken);

        await file.DecompressAsync(cancellationToken);

        return file;
    }

    /// <summary>
    /// Asynchronously sets the value of a file in the database.
    /// </summary>
    /// <param name="value">The value to set.</param>
    /// <param name="filename">The name of the file to set the value for. Defaults to "file.txt".</param>
    /// <param name="safe">Whether to use safe mode when setting the value. Defaults to false.</param>
    /// <param name="timeout">The timeout for the operation in milliseconds. Defaults to null.</param>
    /// <param name="cancellationToken">The cancellation token. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value that was set.</returns>
    public async Task<string?> SetAsync(string? value, string? filename = "file.txt", bool safe = false, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (value?.Length > 0)
        {
            DbFile file = new() { FileName = filename, FileContent = Encoding.UTF8.GetBytes(value), Safe = safe };
            await file.CompressAsync(cancellationToken);
            return await ExecAsync<string>("WJbFiles_Ins", file, timeout, cancellationToken);
        }

        return await Task.FromResult(value);
    }

    /// <summary>
    /// Asynchronously saves a file in the current database.
    /// </summary>
    /// <param name="file">The file to be saved.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    public async Task<T?> SetAsync<T>(DbFile file, int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (file?.FileContent == null || file.FileContent.Length == 0) return await Task.FromResult<T?>(default);

        await file.CompressAsync(cancellationToken);

        return await ExecAsync<T?>("WJbFiles_Ins", file, timeout, cancellationToken);
    }
}

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
}
