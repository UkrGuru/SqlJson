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
        private static string connectionString;

        public static string ConnectionString { set => connectionString = value; }

        public static SqlConnection CreateSqlConnection() => new (connectionString);

        private static object ConvertToParameterValue(object data) => data is string ? data : JsonSerializer.Serialize(data);

        public static int ExecProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecProc(name, data, timeout);
        }
        public static async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        public static int ExecProc(this SqlConnection connection, string name, object data = null, int? timeout = null)
        {
            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", ConvertToParameterValue(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return command.ExecuteNonQuery();
        }
        public static async Task<int> ExecProcAsync(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", ConvertToParameterValue(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public static string FromProc(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc(name, data, timeout);
        }
        public static async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        public static string FromProc(this SqlConnection connection, string name, object data = null, int? timeout = null)
        {
            var jsonResult = new StringBuilder();

            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", ConvertToParameterValue(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            var reader = command.ExecuteReader();

            if (reader.HasRows)
                while (reader.Read())
                    jsonResult.Append(reader.GetString(0));

            reader.Close();

            return jsonResult.ToString();
        }
        public static async Task<string> FromProcAsync(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var jsonResult = new StringBuilder();

            using SqlCommand command = new(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", ConvertToParameterValue(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows)
                while (await reader.ReadAsync(cancellationToken)) 
                    jsonResult.Append(reader.GetString(0));

            await reader.CloseAsync();

            return jsonResult.ToString();
        }

        public static T FromProc<T>(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc<T>(name, data, timeout);
        }
        public static async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }

        public static T FromProc<T>(this SqlConnection connection, string name, object data = null, int? timeout = null)
        {
            var str = connection.FromProc(name, data, timeout);

            return (string.IsNullOrEmpty(str)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(str);
        }
        public static async Task<T> FromProcAsync<T>(this SqlConnection connection, string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            var str = await connection.FromProcAsync(name, data, timeout, cancellationToken);

            return (string.IsNullOrEmpty(str)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(str);
        }

        public static int ExecCommand(string cmdText, int? timeout = null)
        {
            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecCommand(cmdText, timeout);
        }
        public static async Task<int> ExecCommandAsync(string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecCommandAsync(cmdText, timeout, cancellationToken);
        }

        public static int ExecCommand(this SqlConnection connection, string cmdText, int? timeout = null)
        {
            using SqlCommand command = new(cmdText, connection);

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return command.ExecuteNonQuery();
        }
        public static async Task<int> ExecCommandAsync(this SqlConnection connection, string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using SqlCommand command = new(cmdText, connection);

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}