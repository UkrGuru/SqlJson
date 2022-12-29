// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.Extensions;

/// <summary>
/// Additional set of functions for checking null or blank values.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Throws a System.ArgumentNullException if the argument is null, otherwise the returned argument.
    /// </summary>
    /// <param name="argument"></param>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    public static object? ThrowIfNull(this object? argument, string? argumentName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        return argument;
    }
}