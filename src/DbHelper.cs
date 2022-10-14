// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Database Helper minimizes the effort to process or retrieve data from SQL Server databases.
/// </summary>
public static class DbHelper
{
    private static string? _connectionString;

    /// <summary>
    /// Sets the string used to open a SQL Server database.
    /// </summary>
    public static string ConnectionString { set => _connectionString = value; }

    /// <summary>
    /// Initializes a new instance of the Microsoft.Data.SqlClient.SqlConnection class.
    /// </summary>
    public static SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <summary>
    /// Converts a data object to the standard @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static object NormalizeParams(object data)
    {
        switch (data.GetType().Name)
        {
            case "Boolean":
            case "Byte":
            case "Byte[]":
            case "Char[]":
            case "DateTime":
            case "DateTimeOffset":
            case "Decimal":
            case "Double":
            case "Guid":
            case "Int16":
            case "Int32":
            case "Single":
            case "String":
            case "TimeSpan":
            case "Xml":
                return data;
            default:
                return JsonSerializer.Serialize(data);
        }
    }

    /// <summary>
    /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecProc(string name, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.ExecProc(name, data, timeout);
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecProc, which
    /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecProcAsync(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecProc(this SqlConnection connection, string name, object? data = null, int? timeout = null)
    {
        using SqlCommand command = new(name, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", NormalizeParams(data));

        if (timeout != null) command.CommandTimeout = timeout.Value;

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecProc, which
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecProcAsync(this SqlConnection connection, string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlCommand command = new(name, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", NormalizeParams(data));

        if (timeout != null) command.CommandTimeout = timeout.Value;

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The result as object.</returns>
    public static T? FromProc<T>(string name, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.FromProc<T>(name, data, timeout);
    }

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
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The result as object.</returns>
    public static T? FromProc<T>(this SqlConnection connection, string name, object? data = null, int? timeout = null)
    {
        var jsonResult = new StringBuilder();

        using SqlCommand command = new(name, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", NormalizeParams(data));

        if (timeout != null) command.CommandTimeout = timeout.Value;

        var reader = command.ExecuteReader();

        if (reader.HasRows)
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    jsonResult.Append(reader.GetString(0));

        reader.Close();

        return jsonResult.Length == 0 ? default : jsonResult.ToString().ToObj<T>();
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.FromProc, which
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The result as object.</returns>
    public static async Task<T?> FromProcAsync<T>(this SqlConnection connection, string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var jsonResult = new StringBuilder();

        using SqlCommand command = new(name, connection);
        command.CommandType = System.Data.CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", NormalizeParams(data));

        if (timeout != null) command.CommandTimeout = timeout.Value;

        var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (reader.HasRows)
            while (await reader.ReadAsync(cancellationToken))
                if (!(await reader.IsDBNullAsync(0, cancellationToken)))
                    jsonResult.Append(reader.GetString(0));

        await reader.CloseAsync();

        return await Task.FromResult(jsonResult.Length == 0 ? default : jsonResult.ToString().ToObj<T>());
    }

    /// <summary>
    /// Opens a database connection, then executes a Transact-SQL statement
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecCommand(string cmdText, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.ExecCommand(cmdText, timeout);
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecCommand, which
    /// Opens a database connection, then executes a Transact-SQL statement
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecCommandAsync(string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecCommandAsync(cmdText, timeout, cancellationToken);
    }

    /// <summary>
    /// Executes a Transact-SQL statement
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecCommand(this SqlConnection connection, string cmdText, int? timeout = null)
    {
        using SqlCommand command = new(cmdText, connection);

        if (timeout != null) command.CommandTimeout = timeout.Value;

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecProc, which
    /// Executes a Transact-SQL statement
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecCommandAsync(this SqlConnection connection, string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlCommand command = new(cmdText, connection);

        if (timeout != null) command.CommandTimeout = timeout.Value;

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
