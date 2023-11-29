using Microsoft.AspNetCore.Mvc;
using UkrGuru.SqlJson;

namespace ApiHole.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ApiTypeController : ControllerBase
{
    public ApiTypeController(IDbService db) => _db = db;
    private readonly IDbService _db;

    private readonly string _suffix = "_Api";

    [HttpPost("{proc}")]
    public async Task<string?> Create([FromBody] string? data = default)
    {
        try
        {
            var result = ApiDbHelper.DeNormalize(data);
            return await Task.FromResult(ApiDbHelper.Normalize(result));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}");
        }
    }

    [HttpGet("{proc}")]
    public async Task<string?> Read(string? data = default)
    {
        try
        {
            var result = ApiDbHelper.DeNormalize(data);
            return await Task.FromResult(ApiDbHelper.Normalize(result));
        }
        catch (Exception ex)
        {
            return await Task.FromResult($"Error: {ex.Message}");
        }
    }
}