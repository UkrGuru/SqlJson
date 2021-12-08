// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static T ToObj<T>(this string json, JsonSerializerOptions? options = null)
        {
            return (string.IsNullOrEmpty(json)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(json, options);
        }
        public static async Task<T> ToObjAsync<T>(this string json, JsonSerializerOptions? options = null)
        {
            return await Task.FromResult((string.IsNullOrEmpty(json)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(json, options));
        }
    }
}
