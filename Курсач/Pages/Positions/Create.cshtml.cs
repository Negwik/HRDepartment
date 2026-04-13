using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Курсач.Data;  // Было HRApp.Data
using Курсач.Models;  // Было HRApp.Models
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Positions  // Было HRApp.Pages.Positions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Position Position { get; set; } = new Position();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Positions.Add(Position);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Должность успешно добавлена!";
            return RedirectToPage("./Index");
        }
    }
}