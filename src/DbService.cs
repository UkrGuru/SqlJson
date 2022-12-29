// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// 
/// </summary>
public interface IDbService
{
    /// <summary>
    /// 
    /// </summary>
    string ConnectionStringName { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    SqlConnection CreateSqlConnection();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    int Exec(string cmdText, object? data = null, int? timeout = null);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    T? Exec<T>(string cmdText, object? data = null, int? timeout = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> ExecAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> ExecAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
public class DbService : IDbService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly string? _connectionString;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public DbService(IConfiguration configuration)
        => _connectionString = configuration.GetConnectionString(ConnectionStringName);

    /// <summary>
    /// 
    /// </summary>
    public virtual string ConnectionStringName => "DefaultConnection";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public SqlConnection CreateSqlConnection() => new(_connectionString);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public int Exec(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec(cmdText, data, timeout);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public T? Exec<T>(string cmdText, object? data = null, int? timeout = null)
    {
        using SqlConnection connection = CreateSqlConnection();
        connection.Open();

        return connection.Exec<T?>(cmdText, data, timeout);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> ExecAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        using SqlConnection connection = CreateSqlConnection();
        await connection.OpenAsync(cancellationToken);

        return await connection.ExecAsync(cmdText, data, timeout, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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