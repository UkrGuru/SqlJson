// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> CreateAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default) 
        => await ExecAsync<T?>(cmdText, data, timeout, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T?> ReadAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync<T?>(cmdText, data, timeout, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task UpdateAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(cmdText, data, timeout, cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cmdText"></param>
    /// <param name="data"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await ExecAsync(cmdText, data, timeout, cancellationToken);
}
