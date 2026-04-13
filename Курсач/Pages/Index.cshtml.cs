using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;  // Было HRApp.Data

namespace Курсач.Pages  // Было HRApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public int EmployeesCount { get; set; }
        public int DepartmentsCount { get; set; }
        public int PositionsCount { get; set; }

        public async Task OnGetAsync()
        {
            EmployeesCount = await _context.Employees.CountAsync();
            DepartmentsCount = await _context.Departments.CountAsync();
            PositionsCount = await _context.Positions.CountAsync();
        }
    }
}