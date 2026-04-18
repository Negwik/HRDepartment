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
        public Position Position { get; set; } = new Position();

        public List<EmployeeInPosition> Employees { get; set; } = new();

        [BindProperty]
        public int? SelectedEmployeeId { get; set; }

        [BindProperty]
        public int? TargetPositionId { get; set; }

        public SelectList? OtherPositions { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || id == 0)
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Получаем должность из базы данных
            var positionToUpdate = await _context.Positions.FindAsync(id);
            if (positionToUpdate == null)
            {
                return NotFound();
            }

            // Обновляем только название
            positionToUpdate.Title = Position.Title;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Название должности успешно обновлено!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Обработка перевода сотрудника
        public async Task<IActionResult> OnPostMoveEmployeeAsync(int id)
        {
            if (SelectedEmployeeId == null || TargetPositionId == null)
            {
                TempData["ErrorMessage"] = "Выберите сотрудника и должность для перевода";
                return RedirectToPage(new { id });
            }

            var employee = await _context.Employees.FindAsync(SelectedEmployeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Сотрудник не найден";
                return RedirectToPage(new { id });
            }

            var targetPosition = await _context.Positions.FindAsync(TargetPositionId);
            if (targetPosition == null)
            {
                TempData["ErrorMessage"] = "Должность назначения не найдена";
                return RedirectToPage(new { id });
            }

            // Переводим сотрудника
            employee.PositionId = TargetPositionId.Value;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Сотрудник {employee.LastName} {employee.FirstName} переведен на должность \"{targetPosition.Title}\"";

            // Перенаправляем обратно на страницу редактирования текущей должности
            return RedirectToPage(new { id });
        }

        private async Task LoadData()
        {
            if (Position != null && Position.Id > 0)
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

                // Загружаем другие должности для перевода
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