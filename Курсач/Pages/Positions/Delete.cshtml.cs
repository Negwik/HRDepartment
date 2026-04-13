using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Positions
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
        public Position Position { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (position == null)
            {
                return NotFound();
            }

            Position = position;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position != null)
            {
                // Проверяем, есть ли сотрудники с этой должностью
                var hasEmployees = await _context.Employees.AnyAsync(e => e.PositionId == id);
                if (hasEmployees)
                {
                    TempData["ErrorMessage"] = "Нельзя удалить должность, которая есть у сотрудников!";
                    return RedirectToPage("./Index");
                }

                Position = position;
                _context.Positions.Remove(Position);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Должность успешно удалена!";
            }

            return RedirectToPage("./Index");
        }
    }
}