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
    /// <param name="resourceFullName"></param>
    /// <returns></returns>
    public static void ExecResource(this Assembly assembly, string resourceFullName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceFullName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        DbHelper.Exec(reader.ReadToEnd());
    }

    /// <summary>
    /// Extracts and executes sql script from an assembly by resource name.
    /// </summary>
    /// <param name="assembly">Assembly containing the required sql script resource</param>
    /// <param name="resourceFullName"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static async Task ExecResourceAsync(this Assembly assembly, string resourceFullName, int? timeout = null)
    {
        using var stream = assembly.GetManifestResourceStream(resourceFullName);
        ArgumentNullException.ThrowIfNull(stream);

        using StreamReader reader = new(stream);
        await DbHelper.ExecAsync(await reader.ReadToEndAsync(), timeout: timeout);
    }

    /// <summary>
    /// Initializes the current database for UkrGuru extensions
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="resourceFileName"></param>
    /// <returns></returns>
    /// 
    public static bool InitDb(this Assembly? assembly, string resourceFileName = "InitDb.sql")
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblyName = assembly.GetName().Name;
        var assemblyVersion = Convert.ToString(assembly.GetName().Version);
       
        var currectVersion = DbHelper.Exec<string?>(cmd_ver_get, assemblyName) ?? "0.0.0.0";

        if (currectVersion.CompareTo(assemblyVersion) != 0)
        {
            assembly.ExecResource($"{assemblyName}.Resources.{resourceFileName}");

            DbHelper.Exec(cmd_ver_set, new { Name = assemblyName, Value = assemblyVersion });
        }

        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    private static readonly string cmd_ver_get = @"
SELECT TOP 1 [value]
FROM sys.extended_properties
WHERE class = 0 AND class_desc = N'DATABASE' AND [name] = @Data
";

    /// <summary>
    /// 
    /// </summary>
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