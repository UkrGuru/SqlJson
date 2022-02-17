// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Globalization;
using System.Text.Json;

namespace System;

public static class StringExtensions
{
    public static T? ToObj<T>(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return default;

        else if (typeof(T) == typeof(string))
            return (T)(object)value;

        else if (typeof(T) == typeof(Guid))
            return (T)(object)Guid.Parse(value);

        else if (typeof(T).IsClass)
            return JsonSerializer.Deserialize<T>(value);

        else if (typeof(T).IsEnum)
            return (T)Enum.Parse(typeof(T), value);

        else if (typeof(T).IsPrimitive)
            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);

        else
            return (T)Convert.ChangeType(value, typeof(T));
    }

    public static async Task<T?> ToObjAsync<T>(this string? value)
    {
        return await Task.FromResult(value.ToObj<T>());
    }
}