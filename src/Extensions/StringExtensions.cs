﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
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

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (type == typeof(string))
            return (T)(object)value;

        else if (type == typeof(Guid))
            return (T)(object)Guid.Parse(value);

        else if (type.IsClass)
            return JsonSerializer.Deserialize<T>(value);

        else if (type.IsEnum)
            return (T)Enum.Parse(type, value);

        else if (type.IsPrimitive)
            return (T)Convert.ChangeType(value, type, CultureInfo.CurrentCulture);

        else 
            return (T)Convert.ChangeType(value, type);

    }

    public static async Task<T?> ToObjAsync<T>(this string? value)
    {
        return await Task.FromResult(value.ToObj<T>());
    }
}