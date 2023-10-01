// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Class that contains the UkrGuru service extensions.
/// </summary>
public static class SqlJsonServiceCollectionExtensions
{
    /// <summary>
    /// Registers UkrGuru SqlJson services for server side application..
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="connectionString">The connection string used to open the SQL Server database.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJson(this IServiceCollection services, string? connectionString = null)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        DbHelper.ConnectionString = connectionString;

        services.AddScoped<IDbService, DbService>();
 
        return services;
    }

    /// <summary>
    /// Registers UkrGuru Extensions for server side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="logLevel">The level of the log to write</param>
    /// <param name="initDb"></param>
    public static IServiceCollection AddSqlJsonExt(this IServiceCollection services, DbLogLevel logLevel = DbLogLevel.Information, bool initDb = true)
    {
        DbLogHelper.MinDbLogLevel = logLevel;

        if (initDb) Assembly.GetExecutingAssembly().InitDb();

        services.AddScoped<IDbLogService, DbLogService>();
        services.AddScoped<IDbFileService, DbFileService>();

        return services;
    }
}