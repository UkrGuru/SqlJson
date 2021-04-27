using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using UkrGuru.SqlJson;

namespace SqlJsonDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly DbService _db;

        public ContactController(DbService db)
        {
            _db = db;
        }

        // GET: api/<ContactController>
        [HttpGet]
        public async Task<List<Contact>> Get()
        {
            return await _db.FromProcAsync<List<Contact>>("Contacts_List");
        }

        // GET api/<ContactController>/1
        [HttpGet("{id}")]
        public async Task<Contact> Get(int id)
        {
            return await _db.FromProcAsync<Contact>("Contacts_Item", new { Id = id });
        }

        // POST api/<ContactController>
        [HttpPost]
        public async Task<int> Post([FromBody] Contact item)
        {
            return await _db.FromProcAsync<int>("Contacts_Ins", item);
        }

        // PUT api/<ContactController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] Contact item)
        {
            await _db.ExecProcAsync("Contacts_Upd", item);
        }

        // DELETE api/<ContactController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await _db.ExecProcAsync("Contacts_Del", new { Id = id });
        }

        // DbHelper Demo
        // POST api/<ContactController>/PostGet
        [HttpPost("PostGet")]
        public async Task<Contact> PostGet([FromBody] Contact item)
        {
            using SqlConnection connection = new SqlConnection(DbHelper.ConnString);
            await connection.OpenAsync();

            var id = await connection.FromProcAsync<int>("Contacts_Ins", item);

            return await connection.FromProcAsync<Contact>("Contacts_Item", new { Id = id });
        }
    }
}