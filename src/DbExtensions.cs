// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using UkrGuru.Extensions;

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
    internal static bool IsLong(this Type type) => type.IsEnum ? false : type.Name switch
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
        StringBuilder result = new();

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (type.IsLong())
        {
            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            if (reader.HasRows)
                while (reader.Read())
                    if (!reader.IsDBNull(0))
                    {
                        var value = reader.GetValue<T?>(type);
                        if (value != null) return value;

                        result.Append(reader.GetValue(0));
                    }
        }
        else
        {
            return ParseScalar<T?>(command.ExecuteScalar());
        }

        return result.ToObj<T?>();
    }

    /// <summary>
    /// Eexecutes a Transact-SQL statement or stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
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
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(this SqlConnection connection, string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        StringBuilder result = new();

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        await using var command = connection.CreateSqlCommand(tsql, data, timeout);

        if (type.IsLong())
        {
            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
            if (reader.HasRows)
                while (await reader.ReadAsync(cancellationToken))
                    if (!await reader.IsDBNullAsync(0, cancellationToken))
                    {
                        var value = reader.GetValue<T?>(type);
                        if (value != null) return value;

                        result.Append(reader.GetValue(0));
                    }
        }
        else
        {
            return ParseScalar<T?>(await command.ExecuteScalarAsync(cancellationToken));
        }

        return result.ToObj<T?>();
    }

    /// <summary>
    /// Gets a value of the specified type from the SqlDataReader.
    /// </summary>
    /// <typeparam name="T">The type of the value to return.</typeparam>
    /// <param name="reader">The SqlDataReader to get the value from.</param>
    /// <param name="type">The Type of the value to return.</param>
    /// <returns>A value of the specified type from the SqlDataReader.</returns>
    public static T? GetValue<T>(this SqlDataReader reader, Type type)
    {
        if (type == typeof(byte[]))
        {
            return (T?)reader.GetValue(0);
        }
        else if (type == typeof(char[]))
        {
            return (T?)(object)reader.GetSqlChars(0).Value;
        }
        else if (type == typeof(Stream))
        {
            return (T?)(object)reader.GetStream(0);
        }
        else if (type == typeof(TextReader))
        {
            return (T?)(object)reader.GetTextReader(0);
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Parses a scalar value of type T from an object.
    /// </summary>
    /// <typeparam name="T">The type of the scalar value to be parsed.</typeparam>
    /// <param name="value">The object to be parsed.</param>
    /// <param name="defaultValue">The default value to return if the object is null or DBNull.</param>
    /// <returns>The parsed scalar value of type T, or the default value if the object is null or DBNull.</returns>
    internal static T? ParseScalar<T>(object? value, T? defaultValue = default)
    {
        if (value == null || value == DBNull.Value) return defaultValue;

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(DateOnly))
            return (T)(object)DateOnly.FromDateTime((DateTime)value);
        else if (type == typeof(TimeOnly))
            return (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value);
        else if (type.IsEnum)
            return (T)Enum.Parse(type, Convert.ToString(value)!);

        return (T?)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
    }
}