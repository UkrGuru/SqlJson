// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson;

/// <summary>
/// Interface that allows to Create, Read, Update and Delete records in a table.
/// </summary>
public interface ICrudDbService
{
    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T object.</param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Server service that allows to Create, Read, Update and Delete records in a table.
/// </summary>
public class CrudDbService : DbService, ICrudDbService
{
    /// <summary>
    /// The Constructor for the CrudDbService
    /// </summary>
    /// <param name="configuration"></param>
    public CrudDbService(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T object.</param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    public async Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    public async Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);
}
