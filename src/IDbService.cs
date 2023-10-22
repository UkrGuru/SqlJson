﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;

namespace UkrGuru.SqlJson;

/// <summary>
/// The base interface of a database service for processing or retrieving data.
/// </summary>
public interface IDbService
{
    /// <summary>
    /// The connectionString name used to open the SQL Server database.
    /// </summary>
    string ConnectionStringName { get; }

    /// <summary>
    /// Initializes a new instance of the SqlConnection class when given a string that contains the connection string.
    /// </summary>
    /// <returns></returns>
    SqlConnection CreateSqlConnection();

    /// <summary>
    /// Synchronous method that opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    int Exec(string tsql, object? data = null, int? timeout = null);

    /// <summary>
    /// Synchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    T? Exec<T>(string tsql, object? data = null, int? timeout = null);

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecAsync(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="tsql">The text of the query or stored procedure name.</param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task with T object.</returns>
    Task<T?> ExecAsync<T>(string tsql, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task with T object.</returns>
    Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T object.</param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task with T object.</returns>
    Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    Task<int> UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    Task<int> DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="proc"></param>
    ///// <param name="data"></param>
    ///// <param name="timeout"></param>
    ///// <param name="cancellationToken"></param>
    ///// <returns></returns>
    //Task TestAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);
}
