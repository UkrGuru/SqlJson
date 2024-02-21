// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;
using UkrGuru.SqlJson.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Class that contains the UkrGuru service extensions.
/// </summary>
public static class SqlJsonClientServiceCollectionExtensions
{
    /// <summary>
    /// Registers UkrGuru SqlJson services for client side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJsonApi(this IServiceCollection services)
    {
        services.AddScoped<IDbService, ApiDbService>();

        return services;
    }

    /// <summary>
    /// Registers UkrGuru Extensions for client side application.
    /// </summary>
    /// <param name="services">The IServiceCollection argument the ConfigureServices method receives.</param>
    /// <returns>The updated IServiceCollection collection argument the ConfigureServices method receives.</returns>
    public static IServiceCollection AddSqlJsonApiExt(this IServiceCollection services)
    {
        services.AddScoped<IDbLogService, ApiDbLogService>();
        services.AddScoped<IDbFileService, ApiDbFileService>();

        return services;
    }
}