using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages
{
    [Authorize]
    public class EmployeeQualificationModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EmployeeQualificationModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Employee> EmployeesList { get; set; } = new();
        public Employee? SelectedEmployee { get; set; }
        public int? SelectedEmployeeId { get; set; }

        public async Task OnGetAsync(int? employeeId)
        {
            EmployeesList = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .OrderBy(e => e.LastName)
                .ToListAsync();

            if (employeeId.HasValue)
            {
                SelectedEmployeeId = employeeId;
                SelectedEmployee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Position)
                    .Include(e => e.Qualifications)
                    .FirstOrDefaultAsync(e => e.Id == employeeId);
            }
        }

        // Обновление отпуска с типом
        public async Task<IActionResult> OnPostUpdateVacationAsync(int employeeId, DateTime? startDate, DateTime? endDate, VacationType vacationType)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                if (startDate.HasValue)
                    employee.VacationStartDate = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                else
                    employee.VacationStartDate = null;

                if (endDate.HasValue)
                    employee.VacationEndDate = DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc);
                else
                    employee.VacationEndDate = null;

                employee.VacationType = vacationType;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Данные отпуска обновлены";
            }
            return RedirectToPage(new { employeeId });
        }

        // Очистка отпуска
        public async Task<IActionResult> OnGetClearVacationAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                employee.VacationStartDate = null;
                employee.VacationEndDate = null;
                employee.VacationType = VacationType.Annual;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Даты отпуска очищены";
            }
            return RedirectToPage(new { employeeId });
        }

        // Добавление курса
        public async Task<IActionResult> OnPostAddQualificationAsync(int employeeId, string courseName, DateTime startDate, DateTime endDate, string? organization, QualificationStatus status)
        {
            if (string.IsNullOrEmpty(courseName))
            {
                TempData["ErrorMessage"] = "Введите название курса";
                return RedirectToPage(new { employeeId });
            }

            var qualification = new Qualification
            {
                CourseName = courseName,
                StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                Organization = organization,
                Status = status,
                EmployeeId = employeeId
            };

            _context.Qualifications.Add(qualification);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Курс добавлен";

            return RedirectToPage(new { employeeId });
        }

        // Редактирование курса
        public async Task<IActionResult> OnPostEditQualificationAsync(int qId, int employeeId, string courseName, DateTime startDate, DateTime endDate, string? organization, QualificationStatus status)
        {
            var qualification = await _context.Qualifications.FindAsync(qId);
            if (qualification != null)
            {
                qualification.CourseName = courseName;
                qualification.StartDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                qualification.EndDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
                qualification.Organization = organization;
                qualification.Status = status;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Курс успешно обновлен";
            }
            else
            {
                TempData["ErrorMessage"] = "Курс не найден";
            }

            return RedirectToPage(new { employeeId });
        }

        // Удаление курса
        public async Task<IActionResult> OnGetDeleteQualificationAsync(int qId, int employeeId)
        {
            var qualification = await _context.Qualifications.FindAsync(qId);
            if (qualification != null)
            {
                _context.Qualifications.Remove(qualification);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Курс удален";
            }
            else
            {
                TempData["ErrorMessage"] = "Курс не найден";
            }
            return RedirectToPage(new { employeeId });
        }
    }
}