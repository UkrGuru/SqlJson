using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using SqlJsonWebApp.Models;

namespace SqlJsonWebApp.Pages.Contacts
{
    public class IndexModel : PageModel
    {
        private readonly DbService _db;

        public IndexModel(DbService db) => _db = db;

        public List<Contact> Contacts { get; set; }

        public async Task OnGetAsync()
        {
            Contacts = await _db.FromProcAsync<List<Contact>>("Contacts_Lst");
        }
    }
}
