using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Departments
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
        public Department Department { get; set; } = new Department();

        public List<EmployeeInDepartment> Employees { get; set; } = new();

        [BindProperty]
        public int? SelectedEmployeeId { get; set; }

        [BindProperty]
        public int? TargetDepartmentId { get; set; }

        public SelectList? OtherDepartments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            Department = department;

            await LoadData();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Получаем отдел из базы данных
            var departmentToUpdate = await _context.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                return NotFound();
            }

            // Обновляем только название
            departmentToUpdate.Name = Department.Name;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Название отдела успешно обновлено!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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
        public async Task<IActionResult> OnPostMoveEmployeeAsync(int id)
        {
            if (SelectedEmployeeId == null || TargetDepartmentId == null)
            {
                TempData["ErrorMessage"] = "Выберите сотрудника и отдел для перемещения";
                return RedirectToPage(new { id });
            }

            var employee = await _context.Employees.FindAsync(SelectedEmployeeId);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "Сотрудник не найден";
                return RedirectToPage(new { id });
            }

            var targetDepartment = await _context.Departments.FindAsync(TargetDepartmentId);
            if (targetDepartment == null)
            {
                TempData["ErrorMessage"] = "Отдел назначения не найден";
                return RedirectToPage(new { id });
            }

            // Перемещаем сотрудника
            employee.DepartmentId = TargetDepartmentId.Value;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Сотрудник {employee.LastName} {employee.FirstName} перемещен в отдел \"{targetDepartment.Name}\"";

            // Перенаправляем обратно на страницу редактирования текущего отдела
            return RedirectToPage(new { id });
        }

        private async Task LoadData()
        {
            if (Department != null && Department.Id > 0)
            {
                // Загружаем сотрудников этого отдела
                var employeesQuery = await _context.Employees
                    .Where(e => e.DepartmentId == Department.Id)
                    .Include(e => e.Position)
                    .ToListAsync();

                Employees = employeesQuery
                    .Select(e => new EmployeeInDepartment
                    {
                        Id = e.Id,
                        LastName = e.LastName,
                        FirstName = e.FirstName,
                        MiddleName = e.MiddleName,
                        Position = e.Position?.Title ?? "Не указана",
                        Email = e.Email
                    })
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToList();

                // Загружаем другие отделы для перемещения
                OtherDepartments = new SelectList(
                    await _context.Departments
                        .Where(d => d.Id != Department.Id)
                        .OrderBy(d => d.Name)
                        .ToListAsync(),
                    "Id", "Name");
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }

    public class EmployeeInDepartment
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }

        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}