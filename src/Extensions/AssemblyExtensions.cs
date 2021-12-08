// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static void ExecResource(this Assembly assembly, string resourceName)
        {
            assembly.ThrowIfNull(nameof(assembly));
            resourceName.ThrowIfBlank(nameof(resourceName));

            var script = string.Empty;
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new(stream);
            script = reader.ReadToEnd();

            DbHelper.ExecCommand(script);
        }
    }
}