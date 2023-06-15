// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using UkrGuru.SqlJson;

namespace SqlJsonWasmDemo.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiHoleController : ControllerBase
{
    public ApiHoleController(IDbService db) => _db = db;

    private readonly IDbService _db;

    private readonly string _suffix = "_api";

    [HttpPost("{proc}")]
    public async Task<string?> Create(string proc, [FromBody] object? data = null)
    {
        try
        {
            return await _db.CreateAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }

    [HttpGet("{proc}")]
    public async Task<string?> Read(string proc, string? data = null)
    {
        try
        {
            return await _db.ReadAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }


    [HttpPut("{proc}")]
    public async Task<string?> Update(string proc, [FromBody] object? data = null)
    {
        try
        {
            await _db.UpdateAsync($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
        return null;
    }

    [HttpDelete("{proc}")]
    public async Task<string?> Delete(string proc, string? data = null)
    {
        try
        {
            await _db.DeleteAsync($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
        return null;
    }

}