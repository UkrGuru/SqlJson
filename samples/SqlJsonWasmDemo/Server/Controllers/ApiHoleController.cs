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

    [HttpGet("{proc}")]
    public async Task<string?> Get(string proc, string? data = null)
    {
        try
        {
            return await _db.ExecAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }

    [HttpPost("{proc}")]
    public async Task<string?> Post(string proc, [FromBody] object? data = null)
    {
        try
        {
            return await _db.ExecAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}. Proc={proc}";
        }
    }
}