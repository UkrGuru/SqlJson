// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extension methods for working with databases.
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Determines whether a given type is considered "long" for database purposes.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>True if the type is considered "long", false otherwise.</returns>
    public static bool IsLong(this Type? type) => type == null || type.IsEnum ? false : type.Name switch
    {
        // or "SByte" or "UInt16"  or "UInt32" or "UInt64" 
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
    public static SqlCommand CreateSqlCommand(this SqlConnection connection, string tsql, object? data = null, int? timeout = null)
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
        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (type.IsLong())
        {
            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader.Read<T?>(type);
        }
        else
        {
            return command.ExecuteScalar().ToObj<T?>();
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
        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        await using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (type.IsLong())
        {
            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);

            return await reader.ReadAsync<T?>(type, cancellationToken);
        }
        else
        {
            return (await command.ExecuteScalarAsync(cancellationToken)).ToObj<T?>();
        }
    }

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <param name="type">The type of the value to be returned.</param>
    /// <returns>The value casted to type T.</returns>
    public static T? Read<T>(this SqlDataReader reader, Type type)
    {
        StringBuilder result = new();

        if (reader.HasRows)
            while (reader.Read())
                if (!reader.IsDBNull(0))
                {
                    var value = reader.GetValue<T?>(type);
                    if (value != null) return value;

                    result.Append(reader.GetValue(0));
                }

        return result.ToObj<T?>();
    }

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <param name="type">The type of the value to be returned.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The value casted to type T.</returns>
    public static async Task<T?> ReadAsync<T>(this SqlDataReader reader, Type type, CancellationToken cancellationToken)
    {
        StringBuilder result = new();

        if (reader.HasRows)
            while (await reader.ReadAsync(cancellationToken))
                if (!await reader.IsDBNullAsync(0, cancellationToken))
                {
                    var value = reader.GetValue<T?>(type);
                    if (value != null) return await Task.FromResult<T?>(value);

                    result.Append(reader.GetValue(0));
                }

        return await Task.FromResult(result.ToObj<T?>());
    }

    /// <summary>
    /// Gets a value of the specified type from the SqlDataReader.
    /// </summary>
    /// <typeparam name="T">The type of the value to return.</typeparam>
    /// <param name="reader">The SqlDataReader to get the value from.</param>
    /// <param name="type">The Type of the value to return.</param>
    /// <returns>A value of the specified type from the SqlDataReader.</returns>
    public static T? GetValue<T>(this SqlDataReader reader, Type type) => type switch
    {
        _ when type == typeof(byte[]) => (T?)reader.GetValue(0),
        _ when type == typeof(char[]) => (T?)(object)reader.GetSqlChars(0).Value,
        _ when type == typeof(Stream) => (T?)(object)reader.GetStream(0),
        _ when type == typeof(TextReader) => (T?)(object)reader.GetTextReader(0),
        _ => default,
    };

    /// <summary>
    /// Converts the object value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The sql object value to convert.</param>
    /// <param name="defaultValue">The default value to return if the conversion fails.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? ToObj<T>(this object? value, T? defaultValue = default)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;

        if (value is string svalue && string.IsNullOrEmpty(svalue))
            return defaultValue;

        if (value is StringBuilder sb)
            return sb?.Length > 0 ? sb.ToString().ToObj(defaultValue) : defaultValue;

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T?)value;

        else if (type.IsClass)
            return value.JsonDeserialize<T?>();

        else if (type == typeof(Guid))
            return (T?)(object)Guid.Parse(Convert.ToString(value)!);

        else if (type.IsEnum)
            return (T?)Enum.Parse(type, Convert.ToString(value)!);

        if (type == typeof(DateOnly))
            return (T)(object)DateOnly.FromDateTime((DateTime)value);

        else if (type == typeof(TimeOnly))
            return (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value);

        else if (type.IsPrimitive)
            return (T?)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        else
            return (T?)Convert.ChangeType(value, type);
    }

    /// <summary>
    /// Reads the UTF-8 encoded text or parses the text representing a single JSON value into a <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The object value to convert.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? JsonDeserialize<T>(this object value)
    {
        if (value is Stream stream)
            return JsonSerializer.Deserialize<T>(stream);

        return JsonSerializer.Deserialize<T>(Convert.ToString(value)!);
    }
}