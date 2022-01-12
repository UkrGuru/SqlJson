// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace System;

public static class ObjectExtensions
{
    public static T ThrowIfNull<T>(this T argument, string? argumentName)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        return argument;
    }

    public static string? ThrowIfBlank(this string? argument, string? argumentName)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException($"'{argumentName}' cannot be blank.", argumentName);

        return argument;
    }
}
