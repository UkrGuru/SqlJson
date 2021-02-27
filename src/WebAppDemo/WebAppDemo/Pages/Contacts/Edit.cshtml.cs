using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using WebAppDemo.Models;

namespace WebAppDemo.Pages.Contacts
{
    public class EditModel : PageModel
    {
        private readonly DbService _db;

        public EditModel(DbService db)
        {
            _db = db;
        }

        [BindProperty]
        public Contact Contact { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Contact = await _db.FromProcAsync<Contact>("Contacts_Item", new { Id = id });

            if (Contact.Id == 0)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _db.ExecProcAsync("Contacts_Upd", Contact);

            return RedirectToPage("./Index");
        }
    }
}
