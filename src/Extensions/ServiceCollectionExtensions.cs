// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Class that contains the UkrGuru service extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the UkrGuru SqlJson services. You must call it in the ConfigureServices method of the Startup class of your project.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <param name="connectionString">The connection string used to open the SQL Server database.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJson(this IServiceCollection services, string? connectionString = null)
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            DbHelper.ConnectionString = connectionString;

            services.AddScoped<DbService>();
        }

        return services;
    }
}