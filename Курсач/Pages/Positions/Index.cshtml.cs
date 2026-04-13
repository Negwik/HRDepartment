using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Positions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<PositionWithCount> Positions { get; set; } = new();

        public async Task OnGetAsync()
        {
            Positions = await _context.Positions
                .Select(p => new PositionWithCount
                {
                    Id = p.Id,
                    Title = p.Title,
                    EmployeeCount = p.Employees.Count
                })
                .OrderBy(p => p.Title)
                .ToListAsync();
        }
    }

    public class PositionWithCount
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int EmployeeCount { get; set; }
    }
}