// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson.Crud;

/// <summary>
/// 
/// </summary>
public class ApiDbService : IDbService
{
    /// <summary>
    /// 
    /// </summary>
    public virtual string? ApiCrudUri => "ApiCrud";

    private readonly HttpClient _http;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="http"></param>
    public ApiDbService(HttpClient http) => _http = http;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PostAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, null), content, cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
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
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.GetAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PutAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, null), content, cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.DeleteAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, data), cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }
}