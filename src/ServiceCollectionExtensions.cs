// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.Extensions.Data;
using UkrGuru.Extensions.Logging;
using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Client;

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
        services.AddScoped<ICrudDbService, CrudDbService>();

        return services;
    }

    /// <summary>
    /// Registers UkrGuru Extensions for server side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="logLevel"></param>
    public static IServiceCollection AddSqlJsonExt(this IServiceCollection services, DbLogLevel logLevel = DbLogLevel.Information)
    {
        DbLogHelper.MinDbLogLevel = logLevel;

        Assembly.GetExecutingAssembly().InitDb();

        services.AddScoped<IDbLogService, DbLogService>();
        services.AddScoped<IDbFileService, DbFileService>();

        return services;
    }

    /// <summary>
    /// Registers UkrGuru SqlJson services for client side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJsonApi(this IServiceCollection services)
    {
        services.AddScoped<IDbService, ApiDbService>();
        services.AddScoped<ICrudDbService, ApiCrudDbService>();

        return services;
    }

    /// <summary>
    /// Registers UkrGuru Extensions for client side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="logLevel"></param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJsonApiExt(this IServiceCollection services, DbLogLevel logLevel = DbLogLevel.Information)
    {
        services.AddScoped<IDbLogService, ApiDbLogService>();
        services.AddScoped<IDbFileService, ApiDbFileService>();

        return services;
    }
}