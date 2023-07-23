// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;

namespace UkrGuru.SqlJson;

/// <summary>
/// Database service for processing or retrieving data from ApiHole.
/// </summary>
public class ApiDbService : IDbService
{
    /// <summary>
    /// The base pattern of the API.
    /// </summary>
    public virtual string? ApiHolePattern => "ApiHole";

    private readonly HttpClient _http;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiDbService"/> class.
    /// </summary>
    /// <param name="http">The HTTP client.</param>
    public ApiDbService(HttpClient http) => _http = http;

    /// <summary>
    /// NotImplementedException
    /// </summary>
    public string ConnectionStringName => throw new NotImplementedException();

    /// <summary>
    /// NotImplementedException
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public SqlConnection CreateSqlConnection() => throw new NotImplementedException();

    /// <inheritdoc/>
    public int Exec(string tsql, object? data = null, int? timeout = null) => throw new NotImplementedException();

    /// <inheritdoc/>
    public T? Exec<T>(string tsql, object? data = null, int? timeout = null) => throw new NotImplementedException();

    /// <inheritdoc/>
    public async Task<int> ExecAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);
        
        var httpResponse = await _http.PostAsync(ApiHelper.Normalize(ApiHolePattern, proc), ApiHelper.Normalize(data), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> ExecAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); 

        var httpResponse = await _http.GetAsync(ApiHelper.Normalize(ApiHolePattern, proc, data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

        /// <inheritdoc/>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        var httpResponse = await _http.PostAsync(ApiHelper.Normalize(ApiHolePattern, proc), ApiHelper.Normalize(data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        var httpResponse = await _http.GetAsync(ApiHelper.Normalize(ApiHolePattern, proc, data), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        var httpResponse = await _http.PutAsync(ApiHelper.Normalize(ApiHolePattern, proc), ApiHelper.Normalize(data), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiHelper.ValidateProcName(proc);

        var httpResponse = await _http.DeleteAsync(ApiHelper.Normalize(ApiHolePattern, proc, data), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

}