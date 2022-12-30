﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;

namespace UkrGuru.SqlJson.Crud;

/// <summary>
/// 
/// </summary>
public class DbService : SqlJson.DbService, IDbService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public DbService(IConfiguration configuration) : base(configuration) { }

    /// <summary>
    /// Create, or add new entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to create the T object. Important: A stored procedure name can be less than 50 characters long.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns></returns>
    public async Task<T?> CreateAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default) 
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Read, retrieve, search, or view existing entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proc">The name of the stored procedure that will be used to read the T or List<typeparamref name="T"/> object(s). Important: A stored procedure name can be less than 50 characters long.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns></returns>
    public async Task<T?> ReadAsync<T>(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Update, or edit existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to update the T object. Important: A stored procedure name can be less than 50 characters long.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns></returns>
    public async Task UpdateAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);

    /// <summary>
    /// Delete, deactivate, or remove existing entries
    /// </summary>
    /// <param name="proc">The name of the stored procedure that will be used to delete the T object. Important: A stored procedure name can be less than 50 characters long.</param>
    /// <param name="data">The only @Data parameter available for the stored procedure. The data object will be automatically normalized to the standard.</param>
    /// <param name="timeout">The time in seconds to wait for the command to execute. The default is 30 seconds.</param>
    /// <param name="cancellationToken">The cancellation instruction.</param>
    /// <returns></returns>
    public async Task DeleteAsync(string proc, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(proc, data, timeout, cancellationToken);
}
