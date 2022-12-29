// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text.Json;

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
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public static bool IsName(string? cmdText) => (cmdText?.Length > 50) ? false : true; // maybe rewrite in future

    /// <summary>
    /// 
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static bool IsLong(string? typeName) => typeName switch
    {
        "Boolean" or "Byte" or "DateTime" or "DateTimeOffset" or "Decimal" or "Double" or "Guid" or "Int16" or "Int32" or "Single" or "TimeSpan" => false,
        _ => true,
    };

    /// <summary>
    /// Converts a data object to the standard @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static object Normalize(object data)
    {
        return data.GetType().Name switch
        {
            "Boolean" or "Byte" or "Byte[]" or "Char[]" or "DateTime" or "DateTimeOffset" or "Decimal" or "Double" or "Guid" or
                "Int16" or "Int32" or "Single" or "String" or "TimeSpan" or "Xml" => data,
            _ => JsonSerializer.Serialize(data),
        };
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int Exec(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec(cmdText, data, timeout);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    public static T? Exec<T>(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec<T?>(cmdText, data, timeout);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync(cmdText, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync<T?>(cmdText, data, timeout, cancellationToken);
    }
}