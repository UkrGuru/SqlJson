// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Data.SqlClient;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extension methods for working with databases.
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Determines whether the given string is a valid name.
    /// </summary>
    /// <param name="tsql">The string to check.</param>
    /// <returns>True if the string is a valid name; otherwise, false.</returns>
    public static bool IsName(string? tsql) => tsql is not null && tsql.Length <= 100
        && Regex.IsMatch(tsql, @"^([a-zA-Z_]\w*|\[.+?\])(\.([a-zA-Z_]\w*|\[.+?\]))?$");

    /// <summary>
    /// Determines whether the specified type is a long.
    /// </summary>
    /// <typeparam name="T">The type to check.</typeparam>
    /// <returns><c>true</c> if the specified type is a long; otherwise, <c>false</c>.</returns>
    public static bool IsLong<T>() => default(T) switch
    {
        byte or short or int or long or float or double or decimal or
        bool or char or Guid or Enum or DateOnly or DateTime or DateTimeOffset or TimeOnly or TimeSpan or
        SqlByte or SqlInt16 or SqlInt32 or SqlInt64 or SqlSingle or SqlDouble or SqlDecimal or SqlMoney or
        SqlBoolean or SqlGuid or SqlDateTime => false,
        _ => true
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

        if (IsName(tsql)) command.CommandType = CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddData(data);

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    /// <summary>
    /// Adds the specified SqlParameter object to the SqlParameterCollection.
    /// </summary>
    /// <param name="parameters">The parameters of the Transact-SQL statement or stored procedure.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <returns>A new SqlParameter object.</returns>
    public static SqlParameter AddData(this SqlParameterCollection parameters, object data)
    {
        SqlParameter parameter = new("@Data", data);

        if (parameter.SqlValue is null && data is not Enum && data is not Stream && data is not TextReader && data is not XmlReader)
        {
            parameter.SqlValue = JsonSerializer.Serialize(data);
        }

        parameters.Add(parameter);

        return parameter;
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
    public static int Exec(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateSqlCommand(tsql, data, timeout);

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
    public static T? Exec<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default)
    {
        using var command = connection.CreateSqlCommand(tsql, data, timeout);

        object? result;

        if (IsLong<T>())
        {
            using SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            result = reader.ReadAll<T?>();
        }
        else
        {
            result = command.ExecuteScalar();
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
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecAsync(this SqlConnection connection, string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateSqlCommand(tsql, data, timeout);

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
    public static async Task<T?> ExecAsync<T>(this SqlConnection connection, string tsql, object? data = default, int? timeout = default, CancellationToken cancellationToken = default)
    {
        await using var command = connection.CreateSqlCommand(tsql, data, timeout);

        object? result;

        if (IsLong<T>())
        {
            await using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);

            result = await reader.ReadAllAsync<T?>(cancellationToken);
        }
        else
        {
            result = await command.ExecuteScalarAsync(cancellationToken);
        }

        return await Task.FromResult(result.ToObj<T?>());
    }

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <returns>The value casted to type T.</returns>
    public static object? ReadAll<T>(this SqlDataReader reader)
        => reader.Read() && !reader.IsDBNull(0) ? reader.ReadObj<T>() : default(T);

    /// <summary>
    /// Reads the first column of the first row of the result set returned by the query and returns the value casted to type T.
    /// </summary>
    /// <typeparam name="T">The type to which the value should be casted.</typeparam>
    /// <param name="reader">The SqlDataReader instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The value casted to type T.</returns>
    public static async Task<object?> ReadAllAsync<T>(this SqlDataReader reader, CancellationToken cancellationToken = default)
        => ((await reader.ReadAsync(cancellationToken)) && !await reader.IsDBNullAsync(0, cancellationToken)) ?
            await reader.ReadObjAsync<T>(cancellationToken) : await Task.FromResult(default(T));

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
        => typeof(T).Name switch
        {
            "Byte[]" => reader[0],
            "Char[]" => reader.GetSqlChars(0).Value,
            nameof(Stream) => reader.GetStream(0),
            nameof(SqlBinary) => reader.GetSqlBinary(0),
            nameof(SqlBytes) => reader.GetSqlBytes(0),
            nameof(SqlXml) => reader.GetSqlXml(0),
            nameof(SqlChars) => reader.GetSqlChars(0),
            nameof(TextReader) => reader.GetTextReader(0),
            nameof(XmlReader) => reader.GetXmlReader(0),
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
        => await Task.FromResult(typeof(T).Name switch
        {
            "Byte[]" => reader[0],
            "Char[]" => reader.GetSqlChars(0).Value,
            nameof(Stream) => reader.GetStream(0),
            nameof(SqlBinary) => reader.GetSqlBinary(0),
            nameof(SqlBytes) => reader.GetSqlBytes(0),
            nameof(SqlXml) => reader.GetSqlXml(0),
            nameof(SqlChars) => reader.GetSqlChars(0),
            nameof(TextReader) => reader.GetTextReader(0),
            nameof(XmlReader) => reader.GetXmlReader(0),
            _ => await reader.ReadMoreAsync(cancellationToken),
        });

    /// <summary>
    /// Tries to create a new record in the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns></returns>
    public static async Task<string?> TryCreateAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            return await db.CreateAsync<string?>(proc, data);
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// Tries to read a record from the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The record read from the database.</returns>
    public static async Task<string?> TryReadAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            // return Normalize(await db.ReadAsync<object?>(proc, ApiHelper.DeNormalize(data)));
            return await db.ReadAsync<string?>(proc, data);
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string? Normalize(object? data) => (data is null || Convert.IsDBNull(data)) ? null : data switch
    {
        bool => Convert.ToString(data, CultureInfo.InvariantCulture),
        byte or short or int or long or float or double or decimal => Convert.ToString(data, CultureInfo.InvariantCulture),
        DateOnly => ((DateOnly)data).ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTime => ((DateTime)data).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTimeOffset => ((DateTimeOffset)data).ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture),
        TimeOnly => ((TimeOnly)data).ToString("HH:mm:ss", CultureInfo.InvariantCulture),
        TimeSpan => ((TimeSpan)data).ToString("c"),
        Guid or char or string => Convert.ToString(data),
        byte[] => $"0x{Convert.ToHexString((byte[])data)}",
        char[] => new string((char[])data),
        _ => JsonSerializer.Serialize(data)
    };

    /// <summary>
    /// Tries to update a record in the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The number of records updated in the database.</returns>
    public static async Task<string?> TryUpdateAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            return Convert.ToString(await db.UpdateAsync(proc, data));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    /// <summary>
    /// Tries to delete a record from the database.
    /// </summary>
    /// <param name="db">The database service.</param>
    /// <param name="proc">The stored procedure name.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The number of records deleted from the database.</returns>
    public static async Task<string?> TryDeleteAsync(this IDbService db, string proc, string? data = default)
    {
        try
        {
            return Convert.ToString(await db.DeleteAsync(proc, data));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

}