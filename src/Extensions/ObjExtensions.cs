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

        if (value is string svalue && string.IsNullOrEmpty(svalue))
            return defaultValue;

        if (value is StringBuilder sb)
            return sb?.Length > 0 ? sb.ToString().ToObj(defaultValue) : defaultValue;

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T?)value;

        else if (type.IsClass)
            return value.JsonDeserialize<T?>();

        else if (type == typeof(Guid))
            return (T?)(object)Guid.Parse(Convert.ToString(value)!);

        else if (type.IsEnum)
            return (T?)Enum.Parse(type, Convert.ToString(value)!);

        if (type == typeof(DateOnly))
            return (T)(object)DateOnly.FromDateTime((DateTime)value);

        else if (type == typeof(TimeOnly))
            return (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value);

        else if (type.IsPrimitive)
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