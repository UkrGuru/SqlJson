// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Database Helper minimizes the effort to process or retrieve data from SQL Server databases.
/// </summary>
public static partial class DbHelper
{
    private static string? _connectionString;

    /// <summary>
    /// Sets the string used to open a SQL Server database.
    /// </summary>
    public static string ConnectionString { set => _connectionString = value; }

    /// <summary>
    /// Initializes a new instance of the Microsoft.Data.SqlClient.SqlConnection class.
    /// </summary>
    private static SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type"></param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecCommand(string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.ExecCommand(cmdText, data, timeout, type);
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecCommand, which
    /// opens a database connection, then executes a Transact-SQL statement
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecCommandAsync(string cmdText, object? data = null, int? timeout = null,
        CommandType type = CommandType.Text, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecCommandAsync(cmdText, data, timeout, type, cancellationToken);
    }

    /// <summary>
    /// Opens a database connection, then 
    /// executes the stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecProc(string name, object? data = null, int? timeout = null)
        => ExecCommand(name, data, timeout, CommandType.StoredProcedure);

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecProc, which
    /// opens a database connection, then executes the stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecProcAsync(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecCommandAsync(name, data, timeout, CommandType.StoredProcedure, cancellationToken);

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="cmdText">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <returns>The result as object.</returns>
    public static T? FromCommand<T>(string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.FromCommand<T>(cmdText, data, timeout, type);
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.FromCommand, which
    /// Opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="cmdText">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The result as object.</returns>
    public static async Task<T?> FromCommandAsync<T>(string cmdText, object? data = null, int? timeout = null,
        CommandType type = CommandType.Text, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.FromCommandAsync<T>(cmdText, data, timeout, type, cancellationToken);
    }

    /// <summary>
    ///  Opens a database connection, then executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The result as object.</returns>
    public static T? FromProc<T>(string name, object? data = null, int? timeout = null)
        => FromCommand<T>(name, data, timeout, CommandType.StoredProcedure);

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.FromProc, which
    /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The result as object.</returns>
    public static async Task<T?> FromProcAsync<T>(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await FromCommandAsync<T>(name, data, timeout, CommandType.StoredProcedure, cancellationToken);
}
