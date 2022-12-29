// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpResponse"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<T?> ReadAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        httpResponse.EnsureSuccessStatusCode();

        var body = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (body?.StartsWith("Error:") == true)
            throw new HttpRequestException(body.Replace("Error:", "").TrimStart());

        return body.ToObj<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task ReadAsync(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        httpResponse.EnsureSuccessStatusCode();

        var body = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (body?.StartsWith("Error:") == true)
            throw new HttpRequestException(body.Replace("Error:", "").TrimStart());

        await Task.CompletedTask;
    }
}