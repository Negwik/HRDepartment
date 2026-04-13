using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Positions
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Position Position { get; set; } = default!;

        public List<EmployeeInPosition> Employees { get; set; } = new();

        [BindProperty]
        public int? SelectedEmployeeId { get; set; }

        [BindProperty]
        public int? TargetPositionId { get; set; }

        public SelectList? OtherPositions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Positions.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }

            Position = position;

            await LoadData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            _context.Attach(Position).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Название должности успешно обновлено!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(Position.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Обработка перемещения сотрудника
        public async Task<IActionResult> OnPostMoveEmployeeAsync()
        {
            if (SelectedEmployeeId == null || TargetPositionId == null)
            {
                TempData["ErrorMessage"] = "Выберите сотрудника и должность для перемещения";
                await LoadData();
                return Page();
            }

            var employee = await _context.Employees.FindAsync(SelectedEmployeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Сотрудник не найден";
                await LoadData();
                return Page();
            }

            var targetPosition = await _context.Positions.FindAsync(TargetPositionId);
            if (targetPosition == null)
            {
                TempData["ErrorMessage"] = "Должность назначения не найдена";
                await LoadData();
                return Page();
            }

            // Перемещаем сотрудника
            employee.PositionId = TargetPositionId.Value;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Сотрудник {employee.LastName} {employee.FirstName} переведен на должность \"{targetPosition.Title}\"";

            // Обновляем данные
            await LoadData();
            return Page();
        }

        private async Task LoadData()
        {
            if (Position != null)
            {
                // Загружаем сотрудников с этой должностью
                var employeesQuery = await _context.Employees
                    .Where(e => e.PositionId == Position.Id)
                    .Include(e => e.Department)
                    .ToListAsync();

                Employees = employeesQuery
                    .Select(e => new EmployeeInPosition
                    {
                        Id = e.Id,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        MiddleName = e.MiddleName,
                        Department = e.Department?.Name ?? "Не указан",
                        Email = e.Email
                    })
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToList();

                // Загружаем другие должности для перемещения
                OtherPositions = new SelectList(
                    await _context.Positions
                        .Where(p => p.Id != Position.Id)
                        .OrderBy(p => p.Title)
                        .ToListAsync(),
                    "Id", "Title");
            }
        }

        private bool PositionExists(int id)
        {
            return _context.Positions.Any(e => e.Id == id);
        }
    }

    public class EmployeeInPosition
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}