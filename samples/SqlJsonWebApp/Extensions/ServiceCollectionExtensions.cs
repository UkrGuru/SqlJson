// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Reflection;
using UkrGuru.Extensions;
using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebJobsServiceCollectionExtensions
    {
        public static void AddSqlJsonWebApp(this IServiceCollection services, string connectionString)
        {
            connectionString.ThrowIfBlank(nameof(connectionString));

            var dbName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;

            dbName.ThrowIfBlank(nameof(dbName));

            Assembly assembly = Assembly.GetExecutingAssembly();
            var product_name = assembly.GetName().Name;

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
            assembly.ExecResource($"{product_name}.Resources.InitDb.sql");

            DbHelper.ConnectionString = connectionString;
            assembly.ExecResource($"{product_name}.Resources.SeedDb.sql");
        }
    }
}