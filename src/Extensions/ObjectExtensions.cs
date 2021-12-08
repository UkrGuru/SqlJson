// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace System
{
    public static class ObjectExtensions
    {
        public static T ThrowIfNull<T>(this T argument, string argumentName)
        {
            return argument ?? throw new ArgumentNullException(argumentName);
        }

        public static string ThrowIfBlank(this string argument, string argumentName)
        {
            argument.ThrowIfNull(argumentName);

            if (string.IsNullOrWhiteSpace(argument))
                throw new ArgumentException($"'{argumentName}' cannot be blank.", argumentName);

            return argument;
        }
    }
}
