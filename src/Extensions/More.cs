// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.SqlJson.Extensions;

/// <summary>
/// Represents a collection of keys and values.
/// </summary>
public class More : Dictionary<string, object?>
{
    /// <summary>
    /// Deserialize the JSON string into a dictionary, then add the non-existing key/value pairs to the More dictionary.
    /// </summary>
    /// <param name="json">The JSON string to deserialize</param>
    public void AddNew(string? json)
    {
        if (string.IsNullOrEmpty(json)) return;

        var items = JsonSerializer.Deserialize<More>(json);
        if (items == null) return;

        foreach (var item in items.Where(item => !ContainsKey(item.Key)))
            Add(item.Key, item.Value);
    }

    /// <summary>
    /// Gets the string value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get</param>
    /// <returns>The string value associated with the specified key, or null if the key is not found</returns>
    public string? GetValue(string key) => TryGetValue(key, out var value) && value != null ? Convert.ToString(value) : null;

    /// <summary>
    /// Gets the value of the specified type associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to get</typeparam>
    /// <param name="key">The key of the value to get</param>
    /// <param name="defaultValue">The default value to return if the key is not found</param>
    /// <returns>The value of the specified type associated with the specified key, or the default value if the key is not found</returns>
    public T? GetValue<T>(string key, T? defaultValue = default) => GetValue(key).ToObj(defaultValue);
}