using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Departments
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Department Department { get; set; } = default!;
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            Department = department;

            // Получаем сотрудников этого отдела
            Employees = await _context.Employees
                .Include(e => e.Position)
                .Where(e => e.DepartmentId == id)
                .ToListAsync();

            return Page();
        }
    }
}