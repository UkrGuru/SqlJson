// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text;
using System.Text.Json;

namespace UkrGuru.Extensions;

/// <summary>
/// Additional set of functions for converting to an object of a specific type.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Throws a System.ArgumentNullException if the argument is null, 
    /// Throws a System.ArgumentException if the argument is blank, 
    /// otherwise the returned argument.
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string? ThrowIfBlank(this string? argument, string? argumentName)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException($"'{argumentName}' cannot be blank.", argumentName);

        return argument;
    }

    /// <summary>
    /// Converts the StringBuilder value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonResult"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T? ToObj<T>(this StringBuilder? jsonResult, T? defaultValue = default) => jsonResult?.Length > 0 ? jsonResult.ToString().ToObj<T>() : default;

    /// <summary>
    /// Converts the string value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T? ToObj<T>(this string? value, T? defaultValue = default)
    {
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T?)(object)value;

        else if (type == typeof(Guid))
            return (T?)(object)Guid.Parse(value);

        else if (type.IsClass)
            return JsonSerializer.Deserialize<T>(value);

        else if (type.IsEnum)
            return (T?)Enum.Parse(type, value);

        else if (type.IsPrimitive)
            return (T?)Convert.ChangeType(value, type, CultureInfo.CurrentCulture);

        else
            return (T?)Convert.ChangeType(value, type);
    }
}