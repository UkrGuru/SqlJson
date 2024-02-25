// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extentions for working with an API.
/// </summary>
public static class ApiDbExtensions
{
    /// <summary>
    /// Asynchronously executes a stored procedure and returns the number of rows affected or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The result of the stored procedure.</returns>
    public static async Task<int> ExecAsync(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        client.SetTimeout(timeout);

        HttpResponseMessage httpResponse;

        var norm = ApiDbHelper.Normalize(data);

        if (norm?.Length > 1000)
            httpResponse = await client.PostAsync(ApiDbHelper.Normalize(apiHoleUri, proc, null, timeout), new StringContent(norm), cancellationToken);
        else
            httpResponse = await client.GetAsync(ApiDbHelper.Normalize(apiHoleUri, proc, norm, timeout), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a stored procedure and returns the result of type T or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The result of the database operation of type T.</returns>
    public static async Task<T?> ExecAsync<T>(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        HttpResponseMessage httpResponse;

        client.SetTimeout(timeout);

        var norm = ApiDbHelper.Normalize(data);

        if (norm?.Length > 1000)
            httpResponse = await client.PostAsync(ApiDbHelper.Normalize(apiHoleUri, proc, null, timeout), new StringContent(norm), cancellationToken);
        else
            httpResponse = await client.GetAsync(ApiDbHelper.Normalize(apiHoleUri, proc, norm, timeout), cancellationToken);

        return await httpResponse.ReadAsync<T>(cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a stored procedure to create a new entity asynchronously or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The created entity of type T.</returns>
    public static async Task<T?> CreateAsync<T>(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        client.SetTimeout(timeout);

        var norm = ApiDbHelper.Normalize(data); ArgumentNullException.ThrowIfNull(norm, "content");

        var httpResponse = await client.PostAsync(ApiDbHelper.Normalize(apiHoleUri, proc, null, timeout), new StringContent(norm), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a stored procedure to read a entity(ies) or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The entity of type T.</returns>
    public static async Task<T?> ReadAsync<T>(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        client.SetTimeout(timeout);

        var httpResponse = await client.GetAsync(ApiDbHelper.Normalize(apiHoleUri, proc, ApiDbHelper.Normalize(data)), cancellationToken);

        return await httpResponse.ReadAsync<T?>(cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a stored procedure to update a entity(ies) or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The result of the update operation.</returns>
    public static async Task<int> UpdateAsync(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc); ArgumentNullException.ThrowIfNull(data);

        client.SetTimeout(timeout);

        var norm = ApiDbHelper.Normalize(data); ArgumentNullException.ThrowIfNull(norm, "content");

        var httpResponse = await client.PutAsync(ApiDbHelper.Normalize(apiHoleUri, proc, null, timeout), new StringContent(norm), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously executes a stored procedure to delete a entity(ies) or throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="apiHoleUri">The API hole URI.</param>
    /// <param name="proc">The name of the stored procedure.</param>
    /// <param name="data">The data to send (optional).</param>
    /// <param name="timeout">The timeout for the operation (optional).</param>
    /// <param name="cancellationToken">The cancellation token (optional).</param>
    /// <returns>The result of the delete operation.</returns>
    public static async Task<int> DeleteAsync(this HttpClient client, string? apiHoleUri, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    {
        ApiDbHelper.ValidateProcName(proc);

        client.SetTimeout(timeout);

        var norm = ApiDbHelper.Normalize(data);

        var httpResponse = await client.DeleteAsync(ApiDbHelper.Normalize(apiHoleUri, proc, norm, timeout), cancellationToken);

        return await httpResponse.ReadAsync(cancellationToken);
    }

    /// <summary>
    /// Reads the content of an HTTP response as a string and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public static async Task<int> ReadAsync(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        _ = httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        content.ThrowIfError();

        if (int.TryParse(content, out var number)) return await Task.FromResult(number);

        return await Task.FromResult(0);
    }

    /// <summary>
    /// Reads the content of an HTTP response as a string, deserializes it to an object of type <typeparamref name="T"/>, and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">An optional CancellationToken to observe while waiting for the task to complete. Defaults to default(CancellationToken).</param>
    /// <returns>A task that represents the asynchronous read operation and contains the deserialized object.</returns>
    public static async Task<T?> ReadAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        _ = httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        return ApiDbHelper.DeNormalize(content).ToObj<T?>();
    }

    /// <summary>
    /// Sets the timeout for the HTTP client.
    /// </summary>
    /// <param name="client">The HTTP client.</param>
    /// <param name="timeout">The timeout in seconds (optional).</param>
    internal static void SetTimeout(this HttpClient client, int? timeout = default)
    {
        if (timeout > 0) client.Timeout = new TimeSpan(0, 0, (int)timeout + 1);
    }

    /// <summary>
    /// Throws an <see cref="HttpRequestException"/> if the specified content starts with "Error:".
    /// </summary>
    /// <param name="content">The content to check for errors.</param>
    /// <exception cref="HttpRequestException">Thrown if the content starts with "Error:".</exception>
    internal static void ThrowIfError(this string? content)
    {
        if (content?.StartsWith("Error:") == true) throw new HttpRequestException(content["Error:".Length..]?.TrimStart());
    }
}