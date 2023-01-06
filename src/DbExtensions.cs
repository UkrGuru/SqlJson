// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// 
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
    /// <returns>New instance of the SqlCommand class with initialize parameters</returns>
    private static SqlCommand CreateSqlCommand(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null)
    {
        SqlCommand command = new(cmdText, connection);
        if (DbHelper.IsName(cmdText)) command.CommandType = CommandType.StoredProcedure;

        if (data != null) command.Parameters.AddWithValue("@Data", DbHelper.Normalize(data));

        if (timeout.HasValue) command.CommandTimeout = timeout.Value;

        return command;
    }

    /// <summary>
    /// Eexecutes a Transact-SQL statement or stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query or stored procedure. </param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public static int Exec(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null)
    {
        using SqlCommand command = connection.CreateSqlCommand(cmdText, data, timeout);

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Executes a Transact-SQL statement or stored procedure with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query or stored procedure. </param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    public static T? Exec<T>(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null)
    {
        StringBuilder result = new();

        using (var command = connection.CreateSqlCommand(cmdText, data, timeout))
        {
            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (DbHelper.IsLong(type?.Name))
            {
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            result.Append(reader.GetString(0));

                reader.Close();
            }
            else
            {
                result.Append(command.ExecuteScalar());
            }
        }

        return result.ToObj<T?>();
    }

    /// <summary>
    /// Eexecutes a Transact-SQL statement or stored procedure with or without '@Data' parameter 
    /// and returns the number of rows affected.
    /// </summary>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query or stored procedure. </param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> ExecAsync(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlCommand command = connection.CreateSqlCommand(cmdText, data, timeout);

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <summary>
    /// Executes a Transact-SQL statement or stored procedure with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="connection">The connection instance to bind.</param>
    /// <param name="cmdText">The text of the query or stored procedure. </param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    public static async Task<T?> ExecAsync<T>(this SqlConnection connection, string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        StringBuilder result = new();

        using (var command = connection.CreateSqlCommand(cmdText, data, timeout))
        {
            var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (DbHelper.IsLong(type?.Name))
            {
                var reader = await command.ExecuteReaderAsync(cancellationToken);

                if (reader.HasRows)
                    while (await reader.ReadAsync(cancellationToken))
                        if (!await reader.IsDBNullAsync(0, cancellationToken))
                            result.Append(reader.GetString(0));

                await reader.CloseAsync();
            }
            else
            {
                result.Append(await command.ExecuteScalarAsync(cancellationToken));
            }
        }

        return result.ToObj<T?>();
    }
}