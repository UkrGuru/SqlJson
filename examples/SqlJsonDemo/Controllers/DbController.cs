using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UkrGuru.SqlJson;

namespace SqlJsonDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DbController : ControllerBase
    {
        private readonly string _prefix = "api.";

        private readonly DbService _db;
        public DbController(DbService db) => _db = db;

        [HttpGet("{proc}")]
        public async Task<string> Get(string proc, string data = null)
        {
            try
            {
                return await _db.FromProcAsync($"{_prefix}{proc}", data);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"Error: {ex.Message}");
            }
        }

        [HttpPost("{proc}")]
        public async Task<dynamic> Post(string proc, [FromBody] dynamic data = null)
        {
            try
            {
                return await _db.FromProcAsync<dynamic>($"{_prefix}{proc}",
                    (object)data == null ? null : data);
            }
            catch (Exception ex)
            {
                return await Task.FromResult($"Error: {ex.Message}");
            }
        }
    }
}