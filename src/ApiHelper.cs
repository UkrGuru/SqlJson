// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;
using System.Web;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides helper methods for working with APIs.
/// </summary>
public class ApiHelper
{
    /// <summary>
    /// Normalizes the data to be sent to the API.
    /// </summary>
    /// <param name="data">The data to be normalized.</param>
    /// <returns>The normalized data.</returns>
    public static HttpContent? Normalize(object? data)
    {
        if (data == null) return null;

        return data switch
        {
            Stream => new StreamContent((Stream)data),

            TextReader => Normalize(((TextReader)data).ReadToEnd()),

            _ => new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
        };
    }

    /// <summary>
    /// Normalizes the API endpoint URI.
    /// </summary>
    /// <param name="apiHoleUri">The API endpoint URI.</param>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="data">The data to be passed to the stored procedure.</param>
    /// <returns>The normalized API endpoint URI.</returns>
    public static string? Normalize(string? apiHoleUri, string proc, object? data = null)
    {
        var result = proc;

        if (!string.IsNullOrEmpty(apiHoleUri)) result = $"{apiHoleUri}/{result}";

        if (data != null) result += $"?Data={HttpUtility.UrlPathEncode(Convert.ToString(DbHelper.Normalize(data)))}"; // or Uri.EscapeDataString

        return result;
    }

    /// <summary>
    /// Validates the name of a stored procedure.
    /// </summary>
    /// <param name="proc">The name of the stored procedure to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the stored procedure name is invalid.</exception>
    public static void ValidateProcName(string proc)
    {
        if (!DbHelper.IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }
}