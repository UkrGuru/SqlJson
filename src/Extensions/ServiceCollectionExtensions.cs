// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;
using LogLevel = UkrGuru.Extensions.WJbLog.Level;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Class that contains the UkrGuru service extensions.
/// </summary>
public static class UkrGuruSqlJsonExtensions
{
    /// <summary>
    /// Registers the UkrGuru SqlJson services. You must call it in the ConfigureServices method of the Startup class of your project.
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
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    /// <param name="logLevel"></param>
    public static void AddUkrGuruSqlJsonExt(this IServiceCollection services, string? connectionString = null, LogLevel logLevel = LogLevel.Debug)
    {
        services.AddUkrGuruSqlJson(connectionString);

        WJbLogHelper.MinLogLevel = logLevel;

        Assembly.GetExecutingAssembly().InitDb();
    }
}