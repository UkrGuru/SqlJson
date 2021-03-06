﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using WebAppDemo.Models;

namespace WebAppDemo.Pages.Contacts
{
    public class DeleteModel : PageModel
    {
        private readonly DbService _db;

        public DeleteModel(DbService db)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _db.FromProcAsync<Contact>("Contacts_Del", new { Id = id });

            return RedirectToPage("./Index");
        }
    }
}
