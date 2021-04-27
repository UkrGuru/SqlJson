using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UkrGuru.SqlJson;

namespace SqlJsonDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiHoleController : ControllerBase
    {
        private readonly DbService _db;

        public ApiHoleController(DbService db)
        {
            _db = db;
        }

        // GET: <ApiHoleController>/<proc>
        [HttpGet]
        public async Task<string> Get(string proc)
        {
            return await _db.FromProcAsync($"api.{proc}");
        }

        // GET <ApiHoleController>/<proc>/<id>
        [HttpGet("{proc}/{id}")]
        public async Task<string> Get(string proc, string id)
        {
            return await _db.FromProcAsync($"api.{proc}", id);
        }

        // POST <ApiHoleController>/<proc>
        [HttpPost]
        public async Task<string> Post(string proc, [FromBody] string item)
        {
            return await _db.FromProcAsync($"api.{proc}", item);
        }

        // PUT <ApiHoleController>/<proc>/<id>
        [HttpPut("{proc}/{id}")]
        public async Task Put(string proc, string id, [FromBody] string item)
        {
            await _db.ExecProcAsync($"api.{proc}", item);
        }

        // DELETE <ApiHoleController>/<proc>/<id>
        [HttpDelete("{proc}/{id}")]
        public async Task Delete(string proc, string id)
        {
            await _db.ExecProcAsync($"api.{proc}", id);
        }
    }
}