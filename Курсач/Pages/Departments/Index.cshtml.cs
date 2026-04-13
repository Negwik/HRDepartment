using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Departments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DepartmentWithCount> Departments { get; set; } = new();

        public async Task OnGetAsync()
        {
            Departments = await _context.Departments
                .Select(d => new DepartmentWithCount
                {
                    Id = d.Id,
                    Name = d.Name,
                    EmployeeCount = d.Employees.Count
                })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }

    public class DepartmentWithCount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EmployeeCount { get; set; }
    }
}