// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.Extensions;

/// <summary>
/// Represents a collection of keys and values.
/// </summary>
public class More : Dictionary<string, object?> {

    /// <summary>
    /// Deserialize the json in a dictionary, then add the non-existing key/value pairs to the More dictionary.
    /// </summary>
    /// <param name="json"></param>
    public void AddNew(string? json)
    {
        if (string.IsNullOrEmpty(json)) return;

        var items = JsonSerializer.Deserialize<More>(json);

        foreach (var item in items!.Where(item => !ContainsKey(item.Key)))
            Add(item.Key, item.Value);
    }

    /// <summary>
    /// Gets the string value associated with the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string? GetValue(string name) => TryGetValue(name, out var value) && value != null ? Convert.ToString(value) : null;

    /// <summary>
    /// Gets the <typeparamref name="T"/> value associated with the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public T? GetValue<T>(string name, T? defaultValue = default) => GetValue(name).ToObj(defaultValue);
}