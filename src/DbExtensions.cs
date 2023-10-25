// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extension methods for working with databases.
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Determines whether the specified type is a long.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns><c>true</c> if the specified type is a long; otherwise, <c>false</c>.</returns>
    public static bool IsLong<T>() => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)).IsLong();

    /// <summary>
    /// Determines whether a given type is considered "long" for database purposes.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is considered "long", false otherwise.</returns>
    public static bool IsLong(this Type? type) => type == null || type.IsEnum ? false : type.Name switch
    {
        "Boolean" or "Byte" or "Int16" or "Int32" or "Int64" or "Single" or "Double" or "Decimal" or
        "DateOnly" or "DateTime" or "DateTimeOffset" or "TimeOnly" or "TimeSpan" or "Guid" or "Char" => false,
        _ => true,
    };

    /// <summary>
    /// Creates a new instance of the SqlCommand class with initialization parameters.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>New instance of the SqlCommand class with initialize parameters</returns>
    public static SqlCommand CreateSqlCommand(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        SqlCommand command = new(tsql, connection);

        if (DbHelper.IsName(tsql)) command.CommandType = CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", DbHelper.Normalize(data));

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    /// <summary>
    /// Eexecutes a Transact-SQL statement or stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int Exec(this SqlConnection connection, string tsql, object? data = null, int? timeout = null)
    {
        using SqlCommand command = connection.CreateSqlCommand(tsql, data, timeout);

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a Transact-SQL statement or stored procedure with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = null, int? timeout = null)
    {
        using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (IsLong<T?>())
        {
            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader.ReadAll<T?>().ToObj<T?>();
        }
        else
        {
            return command.ExecuteScalar<T?>();
        }
    }

    /// <summary>
    /// Eexecutes a Transact-SQL statement or stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecAsync(this SqlConnection connection, string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await using SqlCommand command = connection.CreateSqlCommand(tsql, data, timeout);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a Transact-SQL statement or stored procedure with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(this SqlConnection connection, string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (IsLong<T?>())
        {
            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);

            return (await reader.ReadAllAsync<T?>(cancellationToken)).ToObj<T?>();
        }
        else
        {
            return await command.ExecuteScalarAsync<T?>(cancellationToken);
        }
    }

    /// <summary>
    /// Executes the query, and returns the first column of the first row in the result set returned by the query as an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="command">The SqlCommand to execute.</param>
    /// <returns>An object of type T that represents the first column of the first row in the result set.</returns>
    public static T? ExecuteScalar<T>(this SqlCommand command)
        => command.ExecuteScalar().ToObj<T?>();

    /// <summary>
    /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query as an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="command">The SqlCommand to execute.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object of type T that represents the first column of the first row in the result set.</returns>
    public static async Task<T?> ExecuteScalarAsync<T>(this SqlCommand command, CancellationToken cancellationToken = default)
        => (await command.ExecuteScalarAsync(cancellationToken)).ToObj<T?>();

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <returns>The value casted to type T.</returns>
    public static object? ReadAll<T>(this SqlDataReader reader)
        => reader.Read() ? reader.ReadObj<T>() : default(T);

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The value casted to type T.</returns>
    public static async Task<object?> ReadAllAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken = default)
        => (await reader.ReadAsync(cancellationToken)) ? await reader.ReadObjAsync<T>(cancellationToken) : await Task.FromResult(default(T));

    /// <summary>
    /// Reads more data from the SqlDataReader and returns it as a string.
    /// </summary>
    /// <param name="reader">The SqlDataReader to read from.</param>
    /// <returns>A string containing the concatenated data.</returns>
    public static object? ReadMore(this SqlDataReader reader)
    {
        StringBuilder sb = new();

        do { sb.Append(reader.GetValue(0)); } while (reader.Read());

        return sb.ToString();
    }

    /// <summary>
    /// Reads more data from the SqlDataReader and returns it as a string asynchronously.
    /// </summary>
    /// <param name="reader">The SqlDataReader to read from.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string containing the concatenated data.</returns>
    public static async Task<object?> ReadMoreAsync(this SqlDataReader reader, CancellationToken cancellationToken = default)
    {
        StringBuilder sb = new();

        do { sb.Append(reader.GetValue(0)); } while (await reader.ReadAsync(cancellationToken));

        return await Task.FromResult(sb.ToString());
    }

    /// <summary>
    /// Reads data from the SqlDataReader and returns it as an object of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="reader">The SqlDataReader to read from.</param>
    /// <returns>An object of type T containing the data read from the SqlDataReader.</returns>
    public static object? ReadObj<T>(this SqlDataReader reader)
        => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)).Name switch
        {
            "Byte[]" => reader[0],
            "Char[]" => reader.GetSqlChars(0).Value,
            nameof(Stream) => reader.GetStream(0),
            nameof(TextReader) => reader.GetTextReader(0),
            _ => reader.ReadMore(),
        };

    /// <summary>
    /// Reads data from the SqlDataReader and returns it as an object of type T asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="reader">The SqlDataReader to read from.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an object of type T containing the data read from the SqlDataReader.</returns>
    public static async Task<object?> ReadObjAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken = default)
        => await Task.FromResult((Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)).Name switch
        {
            "Byte[]" => reader[0],
            "Char[]" => reader.GetSqlChars(0).Value,
            nameof(Stream) => reader.GetStream(0),
            nameof(TextReader) => reader.GetTextReader(0),
            _ => await reader.ReadMoreAsync(cancellationToken),
        });
}