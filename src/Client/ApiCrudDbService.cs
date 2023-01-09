// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using UkrGuru.Extensions;

namespace UkrGuru.SqlJson.Client;

/// <summary>
/// Client service that allows to Create, Read, Update and Delete records in a table.
/// </summary>
public class ApiCrudDbService : ICrudDbService
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
    public ApiCrudDbService(HttpClient http) => _http = http;

    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PostAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, null), content, cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T or List<typeparamref name="T"/> object(s). </param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.GetAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    public async Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PutAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, null), content, cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a query or stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns>The async task.</returns>
    public async Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.DeleteAsync(ApiHelper.BuildRequestUri(ApiCrudUri, proc, data), cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }
}