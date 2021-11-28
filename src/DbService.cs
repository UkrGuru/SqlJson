// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UkrGuru.SqlJson
{
    // Summary:
    //     Database Service minimizes the effort to process or retrieve data from SQL Server databases.
    public class DbService
    {
        private readonly string _connectionString;

        //
        // Summary:
        //     Initializes a new instance of the UkrGuru.SqlJson.DbService class.
        public DbService(IConfiguration configuration) {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
        }

        //
        // Summary:
        //     Connection string key to be used when initializing the UkrGuru.SqlJson.DbService class.
        public virtual string ConnectionStringName => "SqlJsonConnection";

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.Data.SqlClient.SqlConnection class
        public SqlConnection CreateSqlConnection() => new(_connectionString);

        //
        // Summary:
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the number of rows affected.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        // Returns:
        //     The number of rows affected.
        //
        // Remarks:
        //     ## The private variable '_connectionString' is used for connection.
        public int ExecProc(string name, object data = null, int? timeout = null)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecProc(name, data, timeout);
        }

        //
        // Summary:
        //     An asynchronous version of UkrGuru.SqlJson.DbService.ExecProc, which
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the number of rows affected.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        //   cancellationToken:
        //     The cancellation instruction.
        //
        // Returns:
        //     The number of rows affected.
        //
        // Remarks:
        //     ## The private variable '_connectionString' is used for connection.
        public async Task<int> ExecProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecProcAsync(name, data, timeout, cancellationToken);
        }

        //
        // Summary:
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the result as string.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        // Returns:
        //     The result as string.
        //
        // Remarks:
        //     ## The private static variable 'connectionString' is used for connection.
        public string FromProc(string name, object data = null, int? timeout = null)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc(name, data, timeout);
        }

        //
        // Summary:
        //     An asynchronous version of UkrGuru.SqlJson.DbService.FromProc, which
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the result as string.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        //   cancellationToken:
        //     The cancellation instruction.
        //
        // Returns:
        //     The result as string.
        //
        // Remarks:
        //     ## The private static variable 'connectionString' is used for connection.
        public async Task<string> FromProcAsync(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync(name, data, timeout, cancellationToken);
        }

        //
        // Summary:
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the result as object.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        // Returns:
        //     The result as object.
        //
        // Remarks:
        //     ## The private static variable 'connectionString' is used for connection.
        public T FromProc<T>(string name, object data = null, int? timeout = null)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.FromProc<T>(name, data, timeout);
        }

        //
        // Summary:
        //     An asynchronous version of UkrGuru.SqlJson.DbService.FromProc, which
        //     Opens a database connection, then executes the stored procedure with or without '@Data' parameter
        //     and returns the result as object.
        //
        // Parameters:
        //   name:
        //     The name of the stored procedure.
        //     
        //   data:
        //     The single available '@Data' parameter for the stored procedure. The data object will be automatically serialized to json.
        //
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        //   cancellationToken:
        //     The cancellation instruction.
        //
        // Returns:
        //     The result as object.
        //
        // Remarks:
        //     ## The private static variable 'connectionString' is used for connection.
        public async Task<T> FromProcAsync<T>(string name, object data = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            name.ThrowIfBlank(nameof(name));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.FromProcAsync<T>(name, data, timeout, cancellationToken);
        }

        //
        // Summary:
        //     Opens a database connection, then executes a Transact-SQL statement
        //     and returns the number of rows affected.
        //
        // Parameters:
        //   cmdText:
        //     The text of the query.
        //     
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        // Returns:
        //     The number of rows affected.
        //
        // Remarks:
        //     ## The private variable '_connectionString' is used for connection.
        public int ExecCommand(string cmdText, int? timeout = null)
        {
            cmdText.ThrowIfBlank(nameof(cmdText));

            using SqlConnection connection = CreateSqlConnection();
            connection.Open();

            return connection.ExecCommand(cmdText, timeout);
        }

        //
        // Summary:
        //     An asynchronous version of UkrGuru.SqlJson.DbService.ExecCommand, which
        //     Opens a database connection, then executes a Transact-SQL statement
        //     and returns the number of rows affected.
        //
        // Parameters:
        //   cmdText:
        //     The text of the query.
        //     
        //   timeout:
        //     The time in seconds to wait for the command to execute. The default is 30 seconds. 
        //
        //   cancellationToken:
        //     The cancellation instruction.
        //
        // Returns:
        //     The number of rows affected.
        //
        // Remarks:
        //     ## The private variable '_connectionString' is used for connection.
        public async Task<int> ExecCommandAsync(string cmdText, int? timeout = null, CancellationToken cancellationToken = default)
        {
            cmdText.ThrowIfBlank(nameof(cmdText));

            using SqlConnection connection = CreateSqlConnection();
            await connection.OpenAsync(cancellationToken);

            return await connection.ExecCommandAsync(cmdText, timeout, cancellationToken);
        }
    }
}