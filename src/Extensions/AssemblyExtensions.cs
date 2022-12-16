// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.SqlJson;

namespace UkrGuru.Extensions;

/// <summary>
/// Additional feature set for installing sql patches, saved as a resource in this assembly.
/// </summary>
public static class AssemblyExtensions
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
        await using var stream = assembly.GetManifestResourceStream(resourceName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        await DbHelper.ExecCommandAsync(await reader.ReadToEndAsync());
    }

    /// <summary>
    /// Initializes the current database for UkrGuru extensions
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static bool InitDb(this Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblyName = assembly.GetName().Name;
        var assemblyVersion = Convert.ToString(assembly.GetName().Version);

        string? currectVersion = null;

        currectVersion = DbHelper.FromCommand<string?>(cmd_ver_get, assemblyName);

        currectVersion ??= "0.0.0.0";
        if (currectVersion.CompareTo(assemblyVersion) != 0)
        {
            assembly.ExecResource($"{assemblyName}.Resources.InitDb.sql");

            DbHelper.ExecCommand(cmd_ver_set, new { Name = assemblyName, Value = assemblyVersion });
        }

        return true;
    }

    private static readonly string cmd_ver_get = @"
SELECT TOP 1 [value]
FROM sys.extended_properties
WHERE class = 0 AND class_desc = N'DATABASE' AND [name] = @Data
";

    private static readonly string cmd_ver_set = @"
DECLARE @Name nvarchar(100), @Value sql_variant

SELECT @Name = D.[Name], @Value = CAST(D.[Value] AS sql_variant)
FROM OPENJSON(@Data) WITH ([Name] nvarchar(100), [Value] nvarchar(1000)) D

BEGIN TRY 
	EXEC sp_updateextendedproperty @name = @Name, @value = @Value;  
END TRY 
BEGIN CATCH
	EXEC sp_addextendedproperty @name = @Name, @value = @Value;  
END CATCH 
";
}
