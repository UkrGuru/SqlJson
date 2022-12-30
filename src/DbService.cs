// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// IDbService - the base interface of a database service for processing or retrieving data from SQL Server databases.
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
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    int Exec(string cmdText, object? data = null, int? timeout = null);

    /// <summary>
    /// Synchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    T? Exec<T>(string cmdText, object? data = null, int? timeout = null);

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> ExecAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    Task<T?> ExecAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Database service for processing or retrieving data from SQL Server databases.
/// </summary>
public class DbService : IDbService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly string? _connectionString;

    /// <summary>
    /// The Constructor for the DbService
    /// </summary>
    /// <param name="configuration"></param>
    public DbService(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString(ConnectionStringName);

    /// <summary>
    /// The connectionString name used to open the SQL Server database.
    /// </summary>
    public virtual string ConnectionStringName => "DefaultConnection";

    /// <summary>
    /// Initializes a new instance of the SqlConnection class when given a string that contains the connection string.
    /// </summary>
    /// <returns>New instance of the SqlConnection class</returns>
    public SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <summary>
    /// Synchronous method that opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>The number of rows affected.</returns>
    public int Exec(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec(cmdText, data, timeout);
    }

    /// <summary>
    /// Synchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <returns>Result as an object</returns>
    public T? Exec<T>(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec<T?>(cmdText, data, timeout);
    }

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement and returns the number of rows affected.
    /// </summary>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> ExecAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync(cmdText, data, timeout, cancellationToken);
    }

    /// <summary>
    /// Asynchronous method that opens a database connection, then executes a Transact-SQL statement with or without '@Data' parameter
    /// and returns the result as an object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText">The text of the query or stored procedure. Important: any short CmdText less 50 characters is accepted as a stored procedure name.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the parameter standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>Result as an object</returns>
    public async Task<T?> ExecAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync<T?>(cmdText, data, timeout, cancellationToken);
    }
}

/// <summary>
/// 
/// </summary>
public class ApiDbService : IDbService
{
    /// <summary>
    /// 
    /// </summary>
    public virtual string? ApiHoleUri => "ApiHole";

    /// <summary>
    /// 
    /// </summary>
    private readonly HttpClient _http;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="http"></param>
    public ApiDbService(HttpClient http) => _http = http;

    /// <summary>
    /// 
    /// </summary>
    public string ConnectionStringName => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public SqlConnection CreateSqlConnection() => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Exec(string proc, object? data = null, int? timeout = null) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public T? Exec<T>(string proc, object? data = null, int? timeout = null) => throw new NotImplementedException();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExecAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        HttpResponseMessage? httpResponse;

        var sdata = data == null ? null : DbHelper.Normalize(data).ToString();
        if (sdata?.Length > 1000)
        {
            StringContent content = new(sdata, Encoding.UTF8, "application/json");

            httpResponse = await _http.PostAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, null), content, cancellationToken);
        }
        else
        {
            httpResponse = await _http.GetAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, data), cancellationToken);
        }

        await httpResponse.ReadAsync(cancellationToken);

        return 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> ExecAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        HttpResponseMessage? httpResponse;

        var sdata = data == null ? null : DbHelper.Normalize(data).ToString();
        if (sdata?.Length > 1000)
        {
            StringContent content = new(sdata, Encoding.UTF8, "application/json");

            httpResponse = await _http.PostAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, null), content, cancellationToken);
        }
        else
        {
            httpResponse = await _http.GetAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, data), cancellationToken);
        }

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }
}