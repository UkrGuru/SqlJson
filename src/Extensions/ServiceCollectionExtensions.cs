// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace UkrGuru.SqlJson;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    public static void AddSqlJson(this IServiceCollection services, string? connectionString = null)
    {
        if (connectionString != null) DbHelper.ConnectionString = connectionString;

        services.AddScoped<DbService>();
    }
}