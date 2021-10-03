// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    public static class DbHelper
    {
        public static string ConnString { get; set; }

        public static async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new(ConnString);
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        public static async Task<int> ExecProcAsync(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", data is string ? data : JsonSerializer.Serialize(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public static async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new(ConnString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        public static async Task<string> FromProcAsync(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var jsonResult = new StringBuilder();

            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", data is string ? data : JsonSerializer.Serialize(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (reader.HasRows)
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    jsonResult.Append(reader.GetValue(0)?.ToString());
                }
            }
            await reader.CloseAsync();

            return jsonResult.ToString();
        }

        public static async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = new(ConnString);
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }
       
        public static async Task<T> FromProcAsync<T>(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var str = await connection.FromProcAsync(name, data, timeout, cancellationToken);

            return (string.IsNullOrEmpty(str)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(str);
        }
    }
}