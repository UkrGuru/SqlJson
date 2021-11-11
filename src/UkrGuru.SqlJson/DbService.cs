// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    public class DbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration) => _configuration = configuration;

        public virtual string ConnectionStringName => "SqlJsonConnection";

        private string _connectionString => _configuration.GetConnectionString(ConnectionStringName);

        public SqlConnection CreateSqlConnection() => new SqlConnection(_connectionString);

        public int ExecProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection.ExecProc(name, data, timeout);
        }
        public async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        public string FromProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection.FromProc(name, data, timeout);
        }
        public async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        public T FromProc<T>(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection.FromProc<T>(name, data, timeout);
        }
        public async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }
    }
}