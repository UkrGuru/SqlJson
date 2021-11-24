using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using SqlJsonWebApp.Models;

namespace SqlJsonWebApp.Pages.Contacts
{
    public class DetailsModel : PageModel
    {
        private readonly DbService _db;

        public DetailsModel(DbService db)
        {
            _db = db;
        }

        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contact = await _db.FromProcAsync<Contact>("Contacts_Get", new { Id = id });

            if (Contact.Id == 0)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
