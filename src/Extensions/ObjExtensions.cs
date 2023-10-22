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
    public static T? ToObj<T>(this object? value, T? defaultValue = default) =>
        value == null || value == DBNull.Value ? defaultValue :
        value is T t ? t:
        value is string s ? (string.IsNullOrEmpty(s) ? defaultValue : s.ToTypes<T>()) :
        value is StringBuilder sb ? (sb.Length == 0 ? defaultValue : sb.ToString().ToTypes<T>()) :
        value.ToType<T>();

    /// <summary>
    /// Converts an object to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="value">The object to convert.</param>
    /// <returns>The converted object.</returns>
    public static T? ToType<T>(this object value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t.IsClass => JsonSerializer.Deserialize<T?>((Stream)value),
        Type t when t == typeof(Guid) => (T)(object)Guid.Parse(Convert.ToString(value)!),
        Type t when t.IsEnum => (T)Enum.Parse(t, Convert.ToString(value)!),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime((DateTime)value),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.FromTimeSpan((TimeSpan)value),
        _ => (T?)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// Converts a string to a specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="value">The string to convert.</param>
    /// <returns>The converted object.</returns>
    public static T? ToTypes<T>(this string value) => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
    {
        Type t when t.IsClass => JsonSerializer.Deserialize<T?>(value),
        Type t when t == typeof(Guid) => (T)(object)Guid.Parse(value),
        Type t when t.IsEnum => (T)Enum.Parse(t, value),
        Type t when t == typeof(DateOnly) => (T)(object)DateOnly.FromDateTime(DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)),
        Type t when t == typeof(DateTime) => (T)(object)DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture),
        Type t when t == typeof(DateTimeOffset) => (T)(object)DateTimeOffset.ParseExact(value, "yyyy-MM-dd HH:mm:ss.fffffff zzz", CultureInfo.InvariantCulture),
        Type t when t == typeof(TimeOnly) => (T)(object)TimeOnly.ParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture),
        Type t when t == typeof(TimeSpan) => (T)(object)TimeSpan.ParseExact(value, "hh':'mm':'ss", CultureInfo.InvariantCulture),
        _ => (T?)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), CultureInfo.InvariantCulture),
    };
}