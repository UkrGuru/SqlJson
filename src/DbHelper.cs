// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace UkrGuru.SqlJson;

/// <summary>
/// DbHelper minimizes the effort of processing or retrieving data from SQL Server databases.
/// </summary>
public class DbHelper
{
    /// <summary>
    /// The connection used to open the SQL Server database.
    /// </summary>
    private static string? _connectionString;

    /// <summary>
    /// Specifies the string used to open a SQL Server database.
    /// </summary>
    public static string ConnectionString { set => _connectionString = value; }

    /// <summary>
    /// Initializes a new instance of the SqlConnection class.
    /// </summary>
    /// <returns>New instance of the SqlConnection class with initialize parameters</returns>
    private static SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <summary>
    /// Determines whether the given string is a valid name.
    /// </summary>
    /// <param name="tsql">The string to check.</param>
    /// <returns>True if the string is a valid name; otherwise, false.</returns>
    public static bool IsName(string? tsql) => tsql is not null && tsql.Length <= 100 
        && Regex.IsMatch(tsql, @"^([a-zA-Z_]\w*|\[.+?\])(\.([a-zA-Z_]\w*|\[.+?\]))?$");

    /// <summary>
    /// Converts a data object to the standard @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static object Normalize(object data) => data switch
    {
        // or sbyte or ushort or uint or ulong 
        bool or byte or short or int or long or float or double or decimal or
        DateOnly or DateTime or DateTimeOffset or TimeOnly or TimeSpan or Guid or
        char or string or byte[] or char[] or Stream or TextReader => data,

        _ => JsonSerializer.Serialize(data),
    };

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int Exec(string tsql, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec(tsql, data, timeout);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    public static T? Exec<T>(string tsql, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec<T?>(tsql, data, timeout);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecAsync(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync(tsql, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Creates a new record in the database.
    /// </summary>
    /// <typeparam name="T">The type of the record to be created.</typeparam>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="timeout">The command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created record.</returns>
    public static async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Reads a record from the database.
    /// </summary>
    /// <typeparam name="T">The type of the record to be read.</typeparam>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="timeout">The command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The read record.</returns>
    public static async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Updates a record in the database.
    /// </summary>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="timeout">The command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<int> UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Deletes a record from the database.
    /// </summary>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <param name="timeout">The command timeout.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task<int> DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await DbHelper.ExecAsync(proc, data, timeout, cancellationToken);

}