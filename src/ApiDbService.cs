// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

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

    /// <inheritdoc/>
    public async Task<int> ExecAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        HttpResponseMessage httpResponse;

        var norm = ApiDbHelper.Normalize(data);

        if (norm?.Length > 2000)
            httpResponse = await _http.PostAsync(ApiDbHelper.Normalize(ApiHolePattern, proc), new StringContent(norm), cancellationToken);
        else
            httpResponse = await _http.GetAsync(ApiDbHelper.Normalize(ApiHolePattern, proc, norm), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> ExecAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc); 
        
        HttpResponseMessage httpResponse;

        var norm = ApiDbHelper.Normalize(data);

        if (norm?.Length > 2000)
            httpResponse = await _http.PostAsync(ApiDbHelper.Normalize(ApiHolePattern, proc), new StringContent(norm), cancellationToken);
        else
            httpResponse = await _http.GetAsync(ApiDbHelper.Normalize(ApiHolePattern, proc, norm), cancellationToken);

        return await httpResponse.ReadAsync<T>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        var norm = ApiDbHelper.Normalize(data); ArgumentNullException.ThrowIfNull(norm, "content");

        var httpResponse = await _http.PostAsync(ApiDbHelper.Normalize(ApiHolePattern, proc), new StringContent(norm), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        var norm = ApiDbHelper.Normalize(data);

        var httpResponse = await _http.GetAsync(ApiDbHelper.Normalize(ApiHolePattern, proc, norm), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        var norm = ApiDbHelper.Normalize(data); ArgumentNullException.ThrowIfNull(norm, "content");

        var httpResponse = await _http.PutAsync(ApiDbHelper.Normalize(ApiHolePattern, proc), new StringContent(norm), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        var norm = ApiDbHelper.Normalize(data);

        var httpResponse = await _http.DeleteAsync(ApiDbHelper.Normalize(ApiHolePattern, proc, norm), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }
}