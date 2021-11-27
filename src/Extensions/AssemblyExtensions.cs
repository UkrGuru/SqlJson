// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.IO;
using UkrGuru.SqlJson;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static void ExecResource(this Assembly assembly, string filename)
        {
            assembly.ThrowIfNull(nameof(assembly));
            filename.ThrowIfBlank(nameof(filename));

            var script = string.Empty;
            var resourceName = $"{assembly.GetName().Name}.Resources.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new(stream)) { script = reader.ReadToEnd(); }

            DbHelper.ExecCommand(script);
        }
    }
}