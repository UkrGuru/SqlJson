using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using WebAppDemo.Models;

namespace WebAppDemo.Pages.Contacts
{
    public class IndexModel : PageModel
    {
        private readonly DbService _db;

        public IndexModel(DbService db)
        {
            _db = db;
        }

        public List<Contact> Contact { get; set; }

        public async Task OnGetAsync()
        {
            Contact = await _db.FromProcAsync<List<Contact>>("Contacts_List");
        }
    }
}
