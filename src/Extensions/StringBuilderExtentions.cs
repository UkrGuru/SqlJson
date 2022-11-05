// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;

namespace UkrGuru.Extensions;

internal static class StringBuilderExtentions
{
    public static T? ToObj<T>(this StringBuilder? jsonResult, T? defaultValue = default)
    {
        return jsonResult?.Length > 0 ? jsonResult.ToString().ToObj<T>() : default;
    }
}
