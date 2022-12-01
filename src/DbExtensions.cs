// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Database Extensions minimizes the effort to process or retrieve data from SQL Server databases.
/// </summary>
public static class DbExtensions
{
    /// <summary>
    /// Creates a new instance of the SqlCommand class with initialization parameters.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <returns>New instance of the SqlCommand class with initialize parameters</returns>
    private static SqlCommand CreateSqlCommand(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    {
        SqlCommand command = new(cmdText, connection);
        command.CommandType = type;

        if (data != null) command.Parameters.AddWithValue("@Data", DbHelper.NormalizeParams(data));

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    /// <summary>
    /// Executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecCommand(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    {
        using SqlCommand command = connection.CreateSqlCommand(cmdText, data, timeout, type);

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecCommand, which
    /// executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecCommandAsync(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null,
        CommandType type = CommandType.Text, CancellationToken cancellationToken = default)
    {
        using SqlCommand command = connection.CreateSqlCommand(cmdText, data, timeout, type);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int ExecProc(this SqlConnection connection, string name, object? data = null, int? timeout = null)
        => connection.ExecCommand(name, data, timeout, CommandType.StoredProcedure);

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.ExecProc, which
    /// executes the stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecProcAsync(this SqlConnection connection, string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await connection.ExecCommandAsync(name, data, timeout, CommandType.StoredProcedure, cancellationToken);

    /// <summary>
    /// Executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <returns>The result as object.</returns>
    public static T? FromCommand<T>(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null, CommandType type = CommandType.Text)
    {
        var jsonResult = new StringBuilder();

        using SqlCommand command = connection.CreateSqlCommand(cmdText, data, timeout, type);

        var reader = command.ExecuteReader();

        // if (reader.HasRows)
        while (reader.Read())
            if (!reader.IsDBNull(0))
                jsonResult.Append(reader.GetString(0));

        // reader.Close();

        return jsonResult.ToObj<T>();
    }

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.FromCommand, which
    /// Executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="type">The type of the cmdText. The default is Text.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The result as object.</returns>
    public static async Task<T?> FromCommandAsync<T>(this SqlConnection connection, string name, object? data = null, int? timeout = null,
        CommandType type = CommandType.Text, CancellationToken cancellationToken = default)
    {
        var jsonResult = new StringBuilder();

        using SqlCommand command = connection.CreateSqlCommand(name, data, timeout, type);

        var reader = await command.ExecuteReaderAsync(cancellationToken);

        // if (reader.HasRows)
        while (await reader.ReadAsync(cancellationToken))
            if (!await reader.IsDBNullAsync(0, cancellationToken))
                jsonResult.Append(reader.GetString(0));

        // await reader.CloseAsync();

        return jsonResult.ToObj<T>();
    }

    /// <summary>
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The result as object.</returns>
    public static T? FromProc<T>(this SqlConnection connection, string name, object? data = null, int? timeout = null)
        => connection.FromCommand<T>(name, data, timeout, CommandType.StoredProcedure);

    /// <summary>
    /// An asynchronous version of UkrGuru.SqlJson.DbHelper.FromProc, which
    /// Executes the stored procedure with or without '@Data' parameter
    /// and returns the result as object.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="name">The name of the stored procedure.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The result as object.</returns>
    public static async Task<T?> FromProcAsync<T>(this SqlConnection connection, string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await connection.FromCommandAsync<T>(name, data, timeout, CommandType.StoredProcedure, cancellationToken);
}
