// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson;

/// <summary>
/// Database service for processing or retrieving data from SQL Server databases.
/// </summary>
public class DbService : IDbService
{
    /// <summary>
    /// The ConnectionString name used to open the SQL Server database.
    /// </summary>
    public virtual string ConnectionStringName => "DefaultConnection";

    /// <summary>
    /// Specifies the string used to open a SQL Server database.
    /// </summary>
    private readonly string? _connectionString;

    /// <summary>
    /// Initializes a new instance of the DbService class.
    /// </summary>
    /// <param name="configuration">The configuration object used to retrieve the connection string.</param>
    public DbService(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString(ConnectionStringName);

    /// <summary>
    /// Initializes a new instance of the SqlConnection class when given a string that contains the connection string.
    /// </summary>
    /// <returns>New instance of the SqlConnection class</returns>
    public SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <inheritdoc/>
    public async Task<int> ExecAsync(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync(tsql, data, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> ExecAsync<T>(string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> CreateAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> ReadAsync<T>(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(string proc, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);
}