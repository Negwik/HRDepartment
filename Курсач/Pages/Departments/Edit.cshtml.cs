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
        public Department Department { get; set; } = default!;

        public List<EmployeeInDepartment> Employees { get; set; } = new();

        [BindProperty]
        public int? SelectedEmployeeId { get; set; }

        [BindProperty]
        public int? TargetDepartmentId { get; set; }

        public SelectList? OtherDepartments { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadData();
                return Page();
            }

            _context.Attach(Department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Название отдела успешно обновлено!";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(Department.Id))
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
            if (SelectedEmployeeId == null || TargetDepartmentId == null)
            {
                TempData["ErrorMessage"] = "Выберите сотрудника и отдел для перемещения";
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

            var targetDepartment = await _context.Departments.FindAsync(TargetDepartmentId);
            if (targetDepartment == null)
            {
                TempData["ErrorMessage"] = "Отдел назначения не найден";
                await LoadData();
                return Page();
            }

            // Перемещаем сотрудника
            employee.DepartmentId = TargetDepartmentId.Value;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Сотрудник {employee.LastName} {employee.FirstName} перемещен в отдел \"{targetDepartment.Name}\"";

            // Обновляем данные
            await LoadData();
            return Page();
        }

        private async Task LoadData()
        {
            if (Department != null)
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