// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace System;

public static class StringExtensions
{
    public static T? ToObj<T>(this string? json)
    {
        if (string.IsNullOrEmpty(json)) 
            return default;
        else if (typeof(T) == typeof(string)) 
            return (T?)(object)json;
        else
            return JsonSerializer.Deserialize<T>(json);
    }

    public static async Task<T?> ToObjAsync<T>(this string? json)
    {
        return await Task.FromResult(json.ToObj<T>());
    }
}