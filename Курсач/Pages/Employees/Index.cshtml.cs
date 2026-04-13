using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;  // Было HRApp.Data
using Курсач.Models;  // Было HRApp.Models
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Employees  // Было HRApp.Pages.Employees
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Employee> Employees { get; set; } = new List<Employee>();

        public async Task OnGetAsync()
        {
            Employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }
    }
}