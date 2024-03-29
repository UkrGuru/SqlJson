﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.SqlJson.Extensions;

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
    public static string ThrowIfBlank(this string? argument, string? argumentName = default)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException($"'{argumentName ?? nameof(argument)}' cannot be blank.");

        return argument!;
    }
}