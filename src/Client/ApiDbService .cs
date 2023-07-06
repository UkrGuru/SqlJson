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
    /// <param name="http">HTTP client instance</param>
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
    /// <typeparam name="T">The type of results to return.</typeparam>
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
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public async Task<int> ExecAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default) 
        => (await CreateAsync<int?>(proc, data, timeout, cancellationToken)) ?? 0;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public async Task<T?> ExecAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default) 
        => await ReadAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PostAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, null), content, cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T object.</param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task with T object.</returns>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.GetAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public async Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        StringContent content = new(DbHelper.Normalize(data).ToString()!, Encoding.UTF8, "application/json");

        var httpResponse = await _http.PutAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, null), content, cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. </param>
    /// <param name="data">The only @Data parameter of any type available to a stored procedure.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>The async task.</returns>
    public async Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.DeleteAsync(ApiHelper.BuildRequestUri(ApiHoleUri, proc, data), cancellationToken);

        await httpResponse.ReadAsync(cancellationToken);
    }


    ///// <summary>
    ///// Converts a data object to the standard @Data parameter.
    ///// </summary>
    ///// <param name="data">The string or object value to convert.</param>
    ///// <returns>The standard value for the @Data parameter.</returns>
    //public static object? CreateContent(object? data)
    //{
    //    if (data == null) return null;

    //    return data switch
    //    {
    //        Stream => new StreamContent((Stream)data),

    //        TextReader => new StringContent(((TextReader)data).ReadToEnd(), Encoding.UTF8),

    //        _ => new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
    //    };
    //}

}