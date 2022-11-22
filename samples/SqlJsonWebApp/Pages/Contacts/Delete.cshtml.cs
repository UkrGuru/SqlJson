using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using SqlJsonWebApp.Models;

namespace SqlJsonWebApp.Pages.Contacts
{
    public class DeleteModel : PageModel
    {
        private readonly DbService _db;

        public DeleteModel(DbService db) => _db = db;

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contact = await _db.FromProcAsync<Contact>("Contacts_Get", id);

            if (Contact.Id == 0)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _db.ExecProcAsync("Contacts_Del", id);

            return RedirectToPage("./Index");
        }
    }
}
