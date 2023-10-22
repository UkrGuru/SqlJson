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
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <returns>The value casted to type T.</returns>
    public static object? ReadAll<T>(this SqlDataReader reader)
    {
        Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(byte[]))
            return reader.Read() ? reader[0] : default;

        else if (type == typeof(char[]))
            return reader.Read() ? reader.GetSqlChars(0).Value : default;

        else if (type == typeof(Stream))
            return reader.Read() ? reader.GetStream(0) : Stream.Null;

        else if (type == typeof(TextReader))
            return reader.Read() ? reader.GetTextReader(0) : TextReader.Null;

        else
        {
            StringBuilder sb = new();

            while (reader.Read())
                sb.Append(reader.GetValue(0));

            return sb.ToString();
        }
    }

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The value casted to type T.</returns>
    public static async Task<object?> ReadAllAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken = default)
    {
        Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(byte[]))
            return (await reader.ReadAsync(cancellationToken)) ? reader[0] : default;

        else if (type == typeof(char[]))
            return (await reader.ReadAsync(cancellationToken)) ? reader.GetSqlChars(0).Value : default;

        else if (type == typeof(Stream))
            return (await reader.ReadAsync(cancellationToken)) ? reader.GetStream(0) : Stream.Null;

        else if (type == typeof(TextReader))
            return (await reader.ReadAsync(cancellationToken)) ? reader.GetTextReader(0) : TextReader.Null;

        else
        {
            StringBuilder sb = new();

            while (await reader.ReadAsync(cancellationToken))
                sb.Append(reader.GetValue(0));

            return sb.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <returns></returns>
    public static T? ExecuteScalar<T>(this SqlCommand command)
        => command.ExecuteScalar().ToObj<T?>();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> ExecuteScalarAsync<T>(this SqlCommand command, CancellationToken cancellationToken = default)
        => (await command.ExecuteScalarAsync(cancellationToken)).ToObj<T?>();
}