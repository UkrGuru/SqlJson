// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

/// <summary>
/// 
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="resourceName"></param>
    public static void ExecResource(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        DbHelper.ExecCommand(reader.ReadToEnd());
    }
}
