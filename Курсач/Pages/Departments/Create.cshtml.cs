using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Курсач.Data;  // Было HRApp.Data
using Курсач.Models;  // Было HRApp.Models
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Departments  // Было HRApp.Pages.Departments
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
        public Department Department { get; set; } = new Department();

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

            _context.Departments.Add(Department);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Отдел успешно добавлен!";
            return RedirectToPage("./Index");
        }
    }
}