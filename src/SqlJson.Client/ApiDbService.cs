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
        => await _http.ExecAsync(ApiHolePattern, proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> ExecAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await _http.ExecAsync<T?>(ApiHolePattern, proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await _http.CreateAsync<T?>(ApiHolePattern, proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default) 
        => await _http.ReadAsync<T?>(ApiHolePattern, proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await _http.UpdateAsync(ApiHolePattern, proc, data, timeout, cancellationToken);

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await _http.DeleteAsync(ApiHolePattern, proc, data, timeout, cancellationToken);
}