// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace UkrGuru.Extensions;

/// <summary>
/// Additional set of functions for checking null or blank values.
/// </summary>
public static partial class ObjectExtensions
{
    /// <summary>
    /// Throws a System.ArgumentNullException if the argument is null, 
    /// otherwise the returned argument.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument"></param>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    public static T? ThrowIfNull<T>(this T? argument, string? argumentName)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);

        return argument;
    }
}
