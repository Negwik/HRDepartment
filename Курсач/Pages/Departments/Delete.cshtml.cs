using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Departments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            Department = department;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                // Проверяем, есть ли сотрудники в этом отделе
                var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
                if (hasEmployees)
                {
                    TempData["ErrorMessage"] = "Нельзя удалить отдел, в котором есть сотрудники!";
                    return RedirectToPage("./Index");
                }

                Department = department;
                _context.Departments.Remove(Department);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Отдел успешно удален!";
            }

            return RedirectToPage("./Index");
        }
    }
}