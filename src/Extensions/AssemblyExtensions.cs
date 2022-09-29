// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

/// <summary>
/// Additional feature set for installing sql patches, saved as a resource in this assembly.
/// </summary>
public static partial class AssemblyExtensions
{
    /// <summary>
    /// Extracts and executes sql script from an assembly by resource name.
    /// </summary>
    /// <param name="assembly">Assembly containing the required sql script resource</param>
    /// <param name="resourceName"></param>
    public static void ExecResource(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        DbHelper.ExecCommand(reader.ReadToEnd());
    }

    /// <summary>
    /// Extracts and executes sql script asynchronously from an assembly by resource name.
    /// </summary>
    /// <param name="assembly">Assembly containing the required sql script resource</param>
    /// <param name="resourceName"></param>
    public static async Task ExecResourceAsync(this Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        await DbHelper.ExecCommandAsync(await reader.ReadToEndAsync());
    }
}