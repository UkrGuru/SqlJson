// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace System
{
    public static class ObjectExtensions
    {
        public static void ThrowIfNull(this object argument, string argumentName)
        {
            if (argument is null) throw new ArgumentNullException(argumentName);
        }

        public static void ThrowIfBlank(this object argument, string argumentName)
        {
            argument.ThrowIfNull(argumentName);

            if (argument is string && string.IsNullOrEmpty((string)argument))
                throw new ArgumentException($"'{argumentName}' cannot be null or empty.", argumentName);
        }
    }
}
