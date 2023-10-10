// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Provides extension methods for working with object.
/// </summary>
public static class ObjExtensions
{
    /// <summary>
    /// Converts the object value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The sql object value to convert.</param>
    /// <param name="defaultValue">The default value to return if the conversion fails.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? ToObj<T>(this object? value, T? defaultValue = default)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;

        if (value is StringBuilder sb)
        {
            if (sb.Length == 0) return defaultValue;
            value = sb.ToString();
        }

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T?)value;

        if (value is string svalue && string.IsNullOrEmpty(svalue))
            return defaultValue;

        if (type.IsClass)
            return value.JsonDeserialize<T?>();

        else if (type == typeof(Guid))
            return (T?)(object)Guid.Parse(Convert.ToString(value)!);

        else if (type.IsEnum)
            return (T?)Enum.Parse(type, Convert.ToString(value)!);

        if (value is string)
        {
            string s = (string)value;

            if (type == typeof(DateOnly) || type == typeof(DateTime))
            {
                var dt = DateTime.ParseExact(s, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

                if (type == typeof(DateOnly))
                    return (T)(object)DateOnly.FromDateTime(dt);

                return (T)(object)dt;
            }

            else if (type == typeof(DateTimeOffset))
                return (T)(object)DateTimeOffset.ParseExact(s, "yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture);

            else if (type == typeof(TimeOnly))
                return (T)(object)TimeOnly.ParseExact(s, "HH:mm:ss", CultureInfo.InvariantCulture);

            else if (type == typeof(TimeSpan))
                return (T)(object)TimeSpan.ParseExact(s, "hh':'mm':'ss", CultureInfo.InvariantCulture);
        }
        else
        {
            if (type == typeof(DateOnly))
                return (T)(object)DateOnly.FromDateTime((DateTime)value);

            else if (type == typeof(TimeOnly))
                return (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value);
        }

        if (type.IsPrimitive)
            return (T?)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        else
            return (T?)Convert.ChangeType(value, type);
    }

    /// <summary>
    /// Reads the UTF-8 encoded text or parses the text representing a single JSON value into a <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The object value to convert.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? JsonDeserialize<T>(this object value) => value switch
    {
        Stream stream => JsonSerializer.Deserialize<T>(stream),
        string svalue => JsonSerializer.Deserialize<T>(svalue),
        _ => default,
    };
}