﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides extentions for working with an API.
/// </summary>
public static class ApiDbExtensions
{
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
    /// Throws an <see cref="HttpRequestException"/> if the specified content starts with "Error:".
    /// </summary>
    /// <param name="content">The content to check for errors.</param>
    /// <exception cref="HttpRequestException">Thrown if the content starts with "Error:".</exception>
    public static void ThrowIfError(this string? content)
    {
        if (content?.StartsWith("Error:") == true) throw new HttpRequestException(content["Error:".Length..]?.TrimStart());
    }
}