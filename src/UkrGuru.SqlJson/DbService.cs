// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    public class DbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration) => _configuration = configuration;

        private string _connString => _configuration.GetConnectionString("SqlJsonConnection");

        public async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        public async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        public async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }
    }
}