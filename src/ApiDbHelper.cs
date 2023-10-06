// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides helper methods for working with APIs.
/// </summary>
public class ApiDbHelper
{
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

        if (data != null) result += $"?Data={Uri.EscapeDataString(Normalize(data))}";

        return result;
    }

    /// <summary>
    /// Converts a data object to the standard API @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static string Normalize(object data) => data switch
    {
        bool => Convert.ToString(data)!,
        byte or short or int or long or float or double or decimal => Convert.ToString(data, CultureInfo.InvariantCulture)!,
        DateOnly => ((DateOnly)data).ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTime => ((DateTime)data).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTimeOffset => ((DateTimeOffset)data).ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture),
        TimeOnly => ((TimeOnly)data).ToString("HH:mm:ss", CultureInfo.InvariantCulture),
        //TimeSpan => ((TimeSpan)data).ToString("hh':'mm':'ss", CultureInfo.InvariantCulture),
        Guid or char or string => Convert.ToString(data)!,
        //byte[] => $"0x{Convert.ToHexString((byte[])data)}",
        //char[] => new string((char[])data),
        _ => JsonSerializer.Serialize(data)!
    };

    /// <summary>
    /// Validates the name of a stored procedure.
    /// </summary>
    /// <param name="proc">The name of the stored procedure to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the stored procedure name is invalid.</exception>
    public static void ValidateProcName(string proc)
    {
        if (!DbHelper.IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }

    /// <summary>
    /// Converts an object into an HttpContent object.
    /// </summary>
    /// <param name="data">The object to convert.</param>
    /// <returns>The HttpContent object.</returns>
    public static HttpContent? NormalizeContent(object data) => new StringContent(Normalize(data), Encoding.UTF8, "text/plain");  // "application/json"
}