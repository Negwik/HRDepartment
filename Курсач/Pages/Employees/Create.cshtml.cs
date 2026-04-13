using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Employees
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
        public Employee Employee { get; set; } = new Employee();

        public SelectList? Departments { get; set; }
        public SelectList? Positions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSelectLists();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectLists();
                return Page();
            }

            // Конвертируем даты в UTC
            if (Employee.BirthDate != default)
            {
                Employee.BirthDate = DateTime.SpecifyKind(Employee.BirthDate, DateTimeKind.Utc);
            }

            if (Employee.HireDate != default)
            {
                Employee.HireDate = DateTime.SpecifyKind(Employee.HireDate, DateTimeKind.Utc);
            }

            _context.Employees.Add(Employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Сотрудник успешно добавлен!";
            return RedirectToPage("./Index");
        }

        private async Task LoadSelectLists()
        {
            Departments = new SelectList(
                await _context.Departments.OrderBy(d => d.Name).ToListAsync(),
                "Id", "Name");

            Positions = new SelectList(
                await _context.Positions.OrderBy(p => p.Title).ToListAsync(),
                "Id", "Title");
        }
    }
}