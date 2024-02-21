// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// 
/// </summary>
public static class ApiDbHelper
{
    /// <summary>
    /// Determines whether the given string is a valid name.
    /// </summary>
    /// <param name="tsql">The string to check.</param>
    /// <returns>True if the string is a valid name; otherwise, false.</returns>
    public static bool IsName(string? tsql) => tsql is not null && tsql.Length <= 100
        && Regex.IsMatch(tsql, @"^([a-zA-Z_]\w*|\[.+?\])(\.([a-zA-Z_]\w*|\[.+?\]))?$");

    /// <summary>
    /// Converts (2nd level) a data object to the standard API @Data parameter. 
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    public static string? Normalize(object? data) => (data is null || Convert.IsDBNull(data)) ? null : data switch
    {
        bool => Convert.ToString(data, CultureInfo.InvariantCulture),
        byte or short or int or long or float or double or decimal => Convert.ToString(data, CultureInfo.InvariantCulture),
        DateOnly => ((DateOnly)data).ToDateTime(TimeOnly.MinValue).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTime => ((DateTime)data).ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        DateTimeOffset => ((DateTimeOffset)data).ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture),
        TimeOnly => ((TimeOnly)data).ToString("HH:mm:ss", CultureInfo.InvariantCulture),
        TimeSpan => ((TimeSpan)data).ToString("c"),
        Guid or char or string => Convert.ToString(data),
        byte[] => $"0x{Convert.ToHexString((byte[])data)}",
        char[] => new string((char[])data),
        _ => JsonSerializer.Serialize(data)
    };

    /// <summary>
    /// Normalizes the API endpoint URI.
    /// </summary>
    /// <param name="apiHoleUri">The API endpoint URI.</param>
    /// <param name="proc">The stored procedure to execute.</param>
    /// <param name="norm">Normalized data that will be passed to the stored procedure.</param>
    /// <returns>The normalized API endpoint URI.</returns>
    public static string? Normalize(string? apiHoleUri, string proc, string? norm = default)
    {
        var result = Uri.EscapeDataString(proc.ThrowIfBlank());

        if (!string.IsNullOrEmpty(apiHoleUri)) result = $"{apiHoleUri}/{result}";

        if (norm != null) result = $"{result}?Data={Uri.EscapeDataString(norm)}";

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="norm"></param>
    /// <returns></returns>
    public static object? DeNormalize(string? norm)
    {
        if (norm?.StartsWith("0x") == true)
        {
            try { return Convert.FromHexString(norm[2..]); } catch { }
        }
        return norm;
    }

    /// <summary>
    /// Validates the name of a stored procedure.
    /// </summary>
    /// <param name="proc">The name of the stored procedure to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the stored procedure name is invalid.</exception>
    public static void ValidateProcName(string proc)
    {
        if (!IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }
}