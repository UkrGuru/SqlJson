using Microsoft.AspNetCore.Mvc;
using UkrGuru.SqlJson;

namespace ApiHole.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ApiHoleController : ControllerBase
{
    public ApiHoleController(IDbService db) => _db = db;
    private readonly IDbService _db;

    private readonly string _suffix = "_Api";

    [HttpPost("{proc}")]
    public async Task<string?> Create(string proc, [FromBody] string? data = null)
    {
        try
        {
            return await _db.CreateAsync<string?>($"{proc}{_suffix}", data);
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
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
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    [HttpPut("{proc}")]
    public async Task<string?> Update(string proc, [FromBody] string? data = null)
    {
        try
        {
            return Convert.ToString(await _db.UpdateAsync($"{proc}{_suffix}", data));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }

    [HttpDelete("{proc}")]
    public async Task<string?> Delete(string proc, string? data = null)
    {
        try
        {
            return Convert.ToString(await _db.DeleteAsync($"{proc}{_suffix}", data));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}. Proc={proc}");
        }
    }
}