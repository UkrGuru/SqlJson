// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// Database service for processing or retrieving data from ApiHole.
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