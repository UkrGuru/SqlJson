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
    /// 
    /// </summary>
    /// <param name="tsql"></param>
    /// <returns></returns>
    public static bool IsName(string? tsql) => (tsql?.Length > 100) ? false : Regex.IsMatch(tsql!, @"^(\w+|\[.+?\])(\.(\w+|\[.+?\]))?$");

    /// <summary>
    /// Converts a data object to the standard @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static object Normalize(object data)
    {
        return data switch
        {
            // or sbyte or ushort or uint or ulong 
            bool or byte or short or int or long or float or double or decimal or 
            DateOnly or DateTime or DateTimeOffset or TimeOnly or TimeSpan or Guid or
            char or string or byte[] or char[] or Stream or TextReader => data,

            _ => JsonSerializer.Serialize(data),
        };
    }

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
    /// <param name="cancellationToken">The cancellation instruction.</param>
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
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync<T?>(tsql, data, timeout, cancellationToken);
    }
}