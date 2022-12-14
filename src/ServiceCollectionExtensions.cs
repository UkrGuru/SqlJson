// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Class that contains the UkrGuru service extensions.
/// </summary>
public static class UkrGuruSqlJsonExtensions
{
    /// <summary>
    /// Registers UkrGuru SqlJson services.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="connectionString">The connection string used to open the SQL Server database.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddUkrGuruSqlJson(this IServiceCollection services, string? connectionString = null)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        DbHelper.ConnectionString = connectionString;

        services.AddSingleton<DbService>();

        return services;
    }

    /// <summary>
    /// Registers UkrGuru extensions.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="logLevel"></param>
    public static IServiceCollection AddUkrGuruExtensions(this IServiceCollection services, WJbLog.Level logLevel = WJbLog.Level.Debug)
    {
        WJbLogHelper.MinLogLevel = logLevel;

        Assembly.GetExecutingAssembly().InitDb();
        
        return services;
    }
}