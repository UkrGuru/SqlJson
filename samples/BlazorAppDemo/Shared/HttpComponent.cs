// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components;
using UkrGuru.SqlJson.Crud;

namespace BlazorAppDemo.Shared;

/// <summary>
/// 
/// </summary>
public class HttpComponent : ComponentBase
{
    [Inject]
    private IDbService CrudDb { get; set; }

    public async Task<T?> CreateAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
    => await CrudDb.CreateAsync<T?>(cmdText, data, timeout, cancellationToken);

    public async Task<T?> ReadAsync<T>(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.ReadAsync<T?>(cmdText, data, timeout, cancellationToken);

    public async Task UpdateAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.UpdateAsync(cmdText, data, timeout, cancellationToken);

    public async Task DeleteAsync(string cmdText, object? data = null, int? timeout = null, CancellationToken cancellationToken = default)
        => await CrudDb.DeleteAsync(cmdText, data, timeout, cancellationToken);
}