// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using UkrGuru.SqlJson;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SqlJsonServiceCollectionExtensions
    {
        public static void AddSqlJson(this IServiceCollection services, string connString = null)
        {
            DbHelper.ConnString = connString;
            services.AddScoped<DbService>();
        }
    }
}