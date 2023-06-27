// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="HttpResponseMessage"/> class.
/// </summary>
public static class HttpClientExtensions
{
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="http"></param>
    ///// <param name="proc"></param>
    ///// <param name="data"></param>
    ///// <param name="timeout"></param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>The async task.</returns>
    //public static async Task<int> ExecAsync(this HttpClient http, string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    //{
    //    ApiHelper.ValidateProcName(proc);

    //    HttpResponseMessage? httpResponse;

    //    var sdata = data == null ? null as string : DbHelper.Normalize(data).ToString();
    //    if (sdata?.Length > 1000)
    //    {
    //        StringContent content = new(sdata, Encoding.UTF8, "application/json");

    //        httpResponse = await http.PostAsync(, proc, null), content, cancellationToken);
    //    }
    //    else
    //    {
    //        httpResponse = await http.GetAsync(ApiHelper.BuildRequestUri(http.BaseAddress?.ToString(), proc, data), cancellationToken);
    //    }

    //    await httpResponse.ReadAsync(cancellationToken);

    //    return 0;
    //}

    /// <summary>
    /// Reads the content of an HTTP response as a string and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous read operation.</returns>
    public static async Task ReadAsync(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        ThrowIfError(content);
    }

    /// <summary>
    /// Reads the content of an HTTP response as a string, deserializes it to an object of type <typeparamref name="T"/>, and throws an exception if the content starts with "Error:".
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="httpResponse">The HTTP response to read.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous read operation and contains the deserialized object.</returns>
    public static async Task<T?> ReadAsync<T>(this HttpResponseMessage httpResponse, CancellationToken cancellationToken = default)
    {
        httpResponse.EnsureSuccessStatusCode();

        var content = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        ThrowIfError(content);

        return content.ToObj<T>();
    }

    /// <summary>
    /// Throws an <see cref="HttpRequestException"/> if the specified content starts with "Error:".
    /// </summary>
    /// <param name="content">The content to check for errors.</param>
    /// <exception cref="HttpRequestException">Thrown if the content starts with "Error:".</exception>
    public static void ThrowIfError(string? content)
    {
        const string ErrorPrefix = "Error:";

        if (content?.StartsWith(ErrorPrefix) == true)
        {
            var errorMessage = content.Substring(ErrorPrefix.Length).TrimStart();
            throw new HttpRequestException(errorMessage);
        }
    }
}