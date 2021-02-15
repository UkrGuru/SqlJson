// Copyright (c) 2021 Oleksandr Viktor (UkrGuru)

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    public class DbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string _connString => _configuration.GetConnectionString("SqlJsonConnection");

        public async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null)
        {
            using SqlConnection connection = new SqlConnection(_connString);
            await connection.OpenAsync();

            using SqlCommand command = new SqlCommand(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", JsonSerializer.Serialize(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<string> FromProcAsync(string name, object data = null, int? timeout = null)
        {
            var jsonResult = new StringBuilder();

            using SqlConnection connection = new SqlConnection(_connString);
            await connection.OpenAsync();

            using SqlCommand command = new SqlCommand(name, connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            if (data != null) command.Parameters.AddWithValue("@Data", JsonSerializer.Serialize(data));

            if (timeout != null) command.CommandTimeout = timeout.Value;

            var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    jsonResult.Append(reader.GetValue(0).ToString());
                }
            }

            return jsonResult.ToString();
        }

        public async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null)
        {
            var str = await FromProcAsync(name, data, timeout);

            return (string.IsNullOrEmpty(str)) ? Activator.CreateInstance<T>() : JsonSerializer.Deserialize<T>(str);
        }
    }
}
