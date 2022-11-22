// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text.Json;

namespace UkrGuru.Extensions;

/// <summary>
/// Additional set of functions for the More dictionary.
/// </summary>
public static partial class MoreExtensions
{
    /// <summary>
    /// Deserialize the text representing a array of key/value pairs then adds to the More dictionary.
    /// </summary>
    /// <param name="more"></param>
    /// <param name="json"></param>
    public static void AddNew(this More more, string? json)
    {
        if (string.IsNullOrEmpty(json)) return;

        var items = JsonSerializer.Deserialize<More>(json)!;

        foreach (var item in items.Where(item => !more.ContainsKey(item.Key)))
            more.Add(item.Key, item.Value);
    }

    /// <summary>
    /// Gets the string value associated with the specified name.
    /// </summary>
    /// <param name="more"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string? GetValue(this More? more, string name) => more == null ? null : more.TryGetValue(name, out var value) && value != null ? Convert.ToString(value) : null;

    /// <summary>
    /// Gets the <typeparamref name="T"/> value associated with the specified name.
    /// </summary>
    /// <param name="more"></param>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T? GetValue<T>(this More? more, string name, T? defaultValue = default) => more.GetValue(name).ToObj<T?>(defaultValue);
}