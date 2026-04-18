using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;

namespace Курсач.Pages.Analytics
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Общая статистика
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public int TotalPositions { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal TotalSalaryCost { get; set; }

        // Сотрудники по отделам
        public List<DepartmentStat> EmployeesByDepartment { get; set; } = new();

        // Сотрудники по должностям
        public List<PositionStat> EmployeesByPosition { get; set; } = new();

        // Распределение зарплат
        public List<SalaryRangeStat> SalaryDistribution { get; set; } = new();

        // Возрастная структура
        public List<AgeRangeStat> AgeDistribution { get; set; } = new();

        public async Task OnGetAsync()
        {
            var employees = await _context.Employees.ToListAsync();

            // Общая статистика
            TotalEmployees = employees.Count;
            TotalDepartments = await _context.Departments.CountAsync();
            TotalPositions = await _context.Positions.CountAsync();
            AverageSalary = TotalEmployees > 0 ? employees.Average(e => e.Salary) : 0;
            TotalSalaryCost = employees.Sum(e => e.Salary);

            // Сотрудники по отделам
            var departmentStats = await _context.Departments
                .Select(d => new DepartmentStat
                {
                    DepartmentName = d.Name,
                    Count = d.Employees.Count
                })
                .OrderByDescending(d => d.Count)
                .ToListAsync();

            foreach (var item in departmentStats)
            {
                item.Percentage = TotalEmployees > 0 ? (double)item.Count / TotalEmployees * 100 : 0;
            }
            EmployeesByDepartment = departmentStats.Where(d => d.Count > 0).ToList();

            // Сотрудники по должностям
            var positionStats = await _context.Positions
                .Select(p => new PositionStat
                {
                    PositionTitle = p.Title,
                    Count = p.Employees.Count
                })
                .OrderByDescending(p => p.Count)
                .ToListAsync();

            foreach (var item in positionStats)
            {
                item.Percentage = TotalEmployees > 0 ? (double)item.Count / TotalEmployees * 100 : 0;
            }
            EmployeesByPosition = positionStats.Where(p => p.Count > 0).ToList();

            // Распределение зарплат
            var ranges = new[] { 0, 30000, 50000, 70000, 100000, int.MaxValue };
            var rangeNames = new[] { "До 30 000 ₽", "30 000 - 50 000 ₽", "50 000 - 70 000 ₽", "70 000 - 100 000 ₽", "Более 100 000 ₽" };

            for (int i = 0; i < ranges.Length - 1; i++)
            {
                var count = employees.Count(e => e.Salary >= ranges[i] && e.Salary < ranges[i + 1]);
                if (count > 0 || i == ranges.Length - 2)
                {
                    SalaryDistribution.Add(new SalaryRangeStat
                    {
                        Range = rangeNames[i],
                        Count = count,
                        Percentage = TotalEmployees > 0 ? (double)count / TotalEmployees * 100 : 0
                    });
                }
            }

            // Возрастная структура
            var today = DateTime.Today;
            var ageGroups = new[]
            {
                new { Min = 0, Max = 25, Name = "До 25 лет" },
                new { Min = 25, Max = 35, Name = "25-35 лет" },
                new { Min = 35, Max = 45, Name = "35-45 лет" },
                new { Min = 45, Max = 55, Name = "45-55 лет" },
                new { Min = 55, Max = 150, Name = "Старше 55 лет" }
            };

            foreach (var group in ageGroups)
            {
                var count = employees.Count(e =>
                {
                    var age = today.Year - e.BirthDate.Year;
                    if (e.BirthDate.Date > today.AddYears(-age)) age--;
                    return age >= group.Min && age < group.Max;
                });

                AgeDistribution.Add(new AgeRangeStat
                {
                    AgeRange = group.Name,
                    Count = count,
                    Percentage = TotalEmployees > 0 ? (double)count / TotalEmployees * 100 : 0
                });
            }
        }
    }

    public class DepartmentStat
    {
        public string DepartmentName { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class PositionStat
    {
        public string PositionTitle { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class SalaryRangeStat
    {
        public string Range { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class AgeRangeStat
    {
        public string AgeRange { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}