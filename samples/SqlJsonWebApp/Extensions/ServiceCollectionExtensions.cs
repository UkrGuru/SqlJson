// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System.Reflection;
using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebJobsServiceCollectionExtensions
    {
        public static void AddSqlJsonWebApp(this IServiceCollection services, string connectionString)
        {
            var dbName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;

            DbHelper.ConnectionString = connectionString.Replace(dbName, "master");
            Assembly.GetExecutingAssembly().ExecResource("InitDb.sql");

            DbHelper.ConnectionString = connectionString;
            Assembly.GetExecutingAssembly().ExecResource("SeedDb.sql");
        }
    }
}