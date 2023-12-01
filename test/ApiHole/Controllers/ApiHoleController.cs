using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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
    public async Task<string?> Create(string proc, [FromBody] string? data = default)
        => await _db.TryCreateAsync($"{proc}{_suffix}", data);

    [HttpGet("{proc}")]
    public async Task<string?> Read(string proc, string? data = default)
        => await _db.TryReadAsync($"{proc}{_suffix}", data);

    [HttpPut("{proc}")]
    public async Task<string?> Update(string proc, [FromBody] string? data = default)
        => await _db.TryUpdateAsync($"{proc}{_suffix}", data);

    [HttpDelete("{proc}")]
    public async Task<string?> Delete(string proc, string? data = default)
        => await _db.TryDeleteAsync($"{proc}{_suffix}", data);


    [HttpPost]
    public async Task<string?> SaveUserData([FromBody] JsonElement json)
    => await _db.TryCreateAsync<string?>($"Users_Get_Info{_suffix}", json);
}