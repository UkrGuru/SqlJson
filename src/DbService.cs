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
        private readonly string _connectionString;

        public DbService(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
        }

        public virtual string ConnectionStringName => "SqlJsonConnection";

        public SqlConnection CreateSqlConnection() => new(_connectionString);

        public int ExecProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecProc(name, data, timeout);
        }
        public async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        public string FromProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc(name, data, timeout);
        }
        public async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        public T FromProc<T>(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc<T>(name, data, timeout);
        }
        public async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }

        public int ExecCommand(string cmdText, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecCommand(cmdText, timeout);
        }
        public async Task<int> ExecCommandAsync(string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecCommandAsync(cmdText, timeout, cancellationToken);
        }
    }
}