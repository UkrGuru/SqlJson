using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using SqlJsonWebApp.Models;

namespace SqlJsonWebApp.Pages.Contacts
{
    public class CreateModel : PageModel
    {
        private readonly DbService _db;

        public CreateModel(DbService db) => _db = db;

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Contact Contact { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _db.ExecProcAsync("Contacts_Ins", Contact);

            return RedirectToPage("./Index");
        }
    }
}
