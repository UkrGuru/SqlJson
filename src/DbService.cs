// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    /// <summary>
    /// Database Service minimizes the effort to process or retrieve data from SQL Server databases.
    /// </summary>
    public class DbService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the UkrGuru.SqlJson.DbService class.
        /// </summary>
        public DbService(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
        }

        /// <summary>
        /// Connection string key to be used when initializing the UkrGuru.SqlJson.DbService class.
        /// </summary>
        public virtual string ConnectionStringName => "SqlJsonConnection";

        /// <summary>
        /// Initializes a new instance of the Microsoft.Data.SqlClient.SqlConnection class
        /// </summary>
        public SqlConnection CreateSqlConnection() => new(_connectionString);

        /// <summary>
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the number of rows affected.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecProc(string name, object? data = null, int? timeout = null)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecProc(name, data, timeout);
        }

        /// <summary>
        /// An asynchronous version of UkrGuru.SqlJson.DbService.ExecProc, which
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the number of rows affected.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecProcAsync(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        /// <summary>
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the result as string.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The result as string.</returns>
        public string? FromProc(string name, object? data = null, int? timeout = null)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc(name, data, timeout);
        }

        /// <summary>
        /// An asynchronous version of UkrGuru.SqlJson.DbService.FromProc, which
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the result as string.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The result as string.</returns>
        public async Task<string?> FromProcAsync(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        /// <summary>
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the result as object.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The result as object.</returns>
        public T? FromProc<T>(string name, object? data = null, int? timeout = null)
        {
            return FromProc(name, data, timeout).ToObj<T>();
        }

        /// <summary>
        /// An asynchronous version of UkrGuru.SqlJson.DbService.FromProc, which
        /// Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        /// and returns the result as object.
        /// </summary>
        /// <param name="name">The name of the stored procedure.</param>
        /// <param name="data">The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The result as object.</returns>
        public async Task<T?> FromProcAsync<T>(string name, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            return await (await FromProcAsync(name, data, timeout, cancellationToken)).ToObjAsync<T>();
        }

        /// <summary>
        /// Opens a database connection, then executes a Transact-SQL statement
        /// and returns the number of rows affected.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecCommand(string cmdText, int? timeout = null)
        {
            cmdText.ThrowIfBlank(nameof(cmdText));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecCommand(cmdText, timeout);
        }

        /// <summary>
        /// An asynchronous version of UkrGuru.SqlJson.DbService.ExecCommand, which
        /// Opens a database connection, then executes a Transact-SQL statement
        /// and returns the number of rows affected.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> ExecCommandAsync(string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
        {
            cmdText.ThrowIfBlank(nameof(cmdText));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecCommandAsync(cmdText, timeout, cancellationToken);
        }
    }
}