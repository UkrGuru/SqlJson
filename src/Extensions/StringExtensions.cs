// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using System.Text;
using System.Globalization;
using System.Text.Json.Nodes;

namespace UkrGuru.Extensions;

/// <summary>
/// Provides a set of static methods for string operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Throws a System.ArgumentNullException if the argument is null, 
    /// Throws a System.ArgumentException if the argument is blank, 
    /// otherwise the returned argument.
    /// </summary>
    /// <param name="argument">The string to check.</param>
    /// <param name="argumentName">The name of the argument.</param>
    /// <returns>The original argument.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string? ThrowIfBlank(this string? argument, string? argumentName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        if (string.IsNullOrEmpty(argument))
            throw new ArgumentException($"'{argumentName ?? nameof(argument)}' cannot be blank.");

        return argument;
    }

    /// <summary>
    /// Converts the StringBuilder value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The StringBuilder value to convert.</param>
    /// <param name="defaultValue">The default value to return if the conversion fails.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? ToObj<T>(this StringBuilder? value, T? defaultValue = default)
        => value?.Length > 0 ? value.ToString().ToObj(defaultValue) : defaultValue;

    /// <summary>
    /// Converts the string value to an equivalent T object.
    /// </summary>
    /// <typeparam name="T">The type of results to return.</typeparam>
    /// <param name="value">The string value to convert.</param>
    /// <param name="defaultValue">The default value to return if the conversion fails.</param>
    /// <returns>The converted object of type T.</returns>
    public static T? ToObj<T>(this string? value, T? defaultValue = default)
    {
        if (string.IsNullOrEmpty(value))
            return defaultValue;

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T?)(object)value;

        else if (type.IsClass)
            return JsonSerializer.Deserialize<T?>(value);

        else if (type == typeof(Guid))
            return (T?)(object)Guid.Parse(value);

        else if (type.IsEnum)
            return (T?)Enum.Parse(type, value);

        else if (type.IsPrimitive)
            return (T?)Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

        else
            return (T?)Convert.ChangeType(value, type);
    }

    /// <summary>
    /// Converts a string to a JsonNode object.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>A JsonNode object representing the input string.</returns>
    public static JsonNode? ToJsonNode(this string? value)
    {
        if (value == null)
        {
            return null;
        }
        else if (bool.TryParse(value, out bool bResult))
        {
            return bResult;
        }

        try { return JsonNode.Parse(value); }
        catch { }

        return value;
    }
}