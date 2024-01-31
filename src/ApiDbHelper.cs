// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text.Json;
using UkrGuru.SqlJson.Extensions;

namespace UkrGuru.SqlJson;

/// <summary>
/// Provides helper methods for working with APIs.
/// </summary>
public class ApiDbHelper
{
    /// <summary>
    /// Converts a data object to the standard API @Data parameter.
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns></returns>
    public static string? Normalize(object? data = default) => (data is null || Convert.IsDBNull(data)) ? null : Normalize2(data switch
    {
        SqlBoolean => ((SqlBoolean)data).IsNull ? null : ((SqlBoolean)data).Value,
        SqlByte => ((SqlByte)data).IsNull ? null : ((SqlByte)data).Value,
        SqlInt16 => ((SqlInt16)data).IsNull ? null : ((SqlInt16)data).Value,
        SqlInt32 => ((SqlInt32)data).IsNull ? null : ((SqlInt32)data).Value,
        SqlInt64 => ((SqlInt64)data).IsNull ? null : ((SqlInt64)data).Value,
        SqlSingle => ((SqlSingle)data).IsNull ? null : ((SqlSingle)data).Value,
        SqlDouble => ((SqlDouble)data).IsNull ? null : ((SqlDouble)data).Value,
        SqlDecimal => ((SqlDecimal)data).IsNull ? null : ((SqlDecimal)data).Value,
        SqlMoney => ((SqlMoney)data).IsNull ? null : ((SqlMoney)data).Value,
        SqlDateTime => ((SqlDateTime)data).IsNull ? null : ((SqlDateTime)data).Value,
        SqlGuid => ((SqlGuid)data).IsNull ? null : ((SqlGuid)data).Value,
        SqlString => ((SqlString)data).IsNull ? null : ((SqlString)data).Value,
        SqlBinary => ((SqlBinary)data).IsNull ? null : ((SqlBinary)data).Value,
        SqlBytes => ((SqlBytes)data).IsNull ? null : ((SqlBytes)data).Value,
        SqlChars => ((SqlChars)data).IsNull ? null : ((SqlChars)data).Value,
        SqlXml => ((SqlXml)data).IsNull ? null : ((SqlXml)data).Value,
        _ => data
    });

    /// <summary>
    /// Converts (2nd level) a data object to the standard API @Data parameter. 
    /// </summary>
    /// <param name="data">The string or object value to convert.</param>
    /// <returns>The standard value for the @Data parameter.</returns>
    private static string? Normalize2(object? data) => data is null ? null : data switch
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
    /// <param name="type">Result data type.</param>
    /// <returns>The normalized API endpoint URI.</returns>
    public static string? Normalize(string? apiHoleUri, string proc, string? norm = default, byte? type = default)
    {
        var result = Uri.EscapeDataString(proc.ThrowIfBlank());

        if (!string.IsNullOrEmpty(apiHoleUri)) result = $"{apiHoleUri}/{result}";

        if (norm != null) result = $"{result}?Data={Uri.EscapeDataString(norm)}";

        if (type is not null) result += $"{(norm is null ? '?' : '&')}Type={type}";

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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte? GetTypeCode<T>() => typeof(T).Name switch
    {
        "Byte[]" or nameof(SqlBytes) or nameof(SqlBinary) => (byte?)SqlDbType.VarBinary,
        "Char[]" or nameof(SqlChars) => (byte?)SqlDbType.VarChar,
        nameof(SqlXml) => (byte?)SqlDbType.Xml,
        _ => default,
    };


    /// <summary>
    /// Validates the name of a stored procedure.
    /// </summary>
    /// <param name="proc">The name of the stored procedure to validate.</param>
    /// <exception cref="ArgumentException">Thrown if the stored procedure name is invalid.</exception>
    public static void ValidateProcName(string proc)
    {
        if (!DbExtensions.IsName(proc)) throw new ArgumentException("Invalid stored procedure name.");
    }
}