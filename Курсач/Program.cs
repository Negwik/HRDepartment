using Курсач.Data;
using Курсач.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Настройки пароля - упрощены для тестирования
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Настройки пользователя
    options.User.RequireUniqueEmail = true;

    // Настройки входа
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Перенаправление на страницу входа для неавторизованных пользователей
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated &&
        !context.Request.Path.StartsWithSegments("/Auth/Login") &&
        !context.Request.Path.StartsWithSegments("/css") &&
        !context.Request.Path.StartsWithSegments("/js") &&
        !context.Request.Path.StartsWithSegments("/lib") &&
        !context.Request.Path.StartsWithSegments("/images"))
    {
        context.Response.Redirect("/Auth/Login");
        return;
    }
    await next();
});

app.MapRazorPages();

// Создаем тестовых пользователей, отделы и должности при запуске
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Создаем роль Admin, если ее нет
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
        Console.WriteLine("Роль Admin создана");
    }

    // Создаем роль User, если ее нет
    if (!await roleManager.RoleExistsAsync("User"))
    {
        await roleManager.CreateAsync(new IdentityRole("User"));
        Console.WriteLine("Роль User создана");
    }

    // Удаляем старых пользователей если есть
    var oldAdmin = await userManager.FindByNameAsync("Admin");
    if (oldAdmin != null)
    {
        await userManager.DeleteAsync(oldAdmin);
        Console.WriteLine("Старый Admin удален");
    }

    var oldUser = await userManager.FindByNameAsync("User");
    if (oldUser != null)
    {
        await userManager.DeleteAsync(oldUser);
        Console.WriteLine("Старый User удален");
    }

    // Создаем администратора
    var newAdmin = new User
    {
        UserName = "Admin",
        Email = "admin@example.com",
        FullName = "Администратор системы",
        CreatedAt = DateTime.UtcNow
    };
    var resultAdmin = await userManager.CreateAsync(newAdmin, "1");
    if (resultAdmin.Succeeded)
    {
        await userManager.AddToRoleAsync(newAdmin, "Admin");
        Console.WriteLine("Пользователь Admin создан с паролем 1");
    }
    else
    {
        foreach (var error in resultAdmin.Errors)
        {
            Console.WriteLine($"Ошибка создания Admin: {error.Description}");
        }
    }

    // Создаем обычного пользователя
    var newUser = new User
    {
        UserName = "User",
        Email = "user@example.com",
        FullName = "Обычный пользователь",
        CreatedAt = DateTime.UtcNow
    };
    var resultUser = await userManager.CreateAsync(newUser, "1");
    if (resultUser.Succeeded)
    {
        await userManager.AddToRoleAsync(newUser, "User");
        Console.WriteLine("Пользователь User создан с паролем 1");
    }
    else
    {
        foreach (var error in resultUser.Errors)
        {
            Console.WriteLine($"Ошибка создания User: {error.Description}");
        }
    }

    // Добавляем начальные отделы, если их нет
    if (!context.Departments.Any())
    {
        var departments = new[]
        {
            new Department { Name = "IT-отдел" },
            new Department { Name = "Бухгалтерия" },
            new Department { Name = "Отдел кадров" },
            new Department { Name = "Отдел продаж" },
            new Department { Name = "Маркетинг" },
            new Department { Name = "Администрация" },
            new Department { Name = "Юридический отдел" },
            new Department { Name = "Отдел разработки" }
        };
        await context.Departments.AddRangeAsync(departments);
        await context.SaveChangesAsync();
        Console.WriteLine("Добавлены начальные отделы");
    }

    // Добавляем начальные должности, если их нет
    if (!context.Positions.Any())
    {
        var positions = new[]
        {
            new Position { Title = "Программист" },
            new Position { Title = "Старший программист" },
            new Position { Title = "Ведущий программист" },
            new Position { Title = "Тестировщик" },
            new Position { Title = "Системный администратор" },
            new Position { Title = "Бухгалтер" },
            new Position { Title = "Главный бухгалтер" },
            new Position { Title = "Менеджер по персоналу" },
            new Position { Title = "Менеджер по продажам" },
            new Position { Title = "Маркетолог" },
            new Position { Title = "Директор" },
            new Position { Title = "Заместитель директора" },
            new Position { Title = "Юрист" },
            new Position { Title = "Аналитик" },
            new Position { Title = "Дизайнер" }
        };
        await context.Positions.AddRangeAsync(positions);
        await context.SaveChangesAsync();
        Console.WriteLine("Добавлены начальные должности");
    }

    // Добавляем тестовых сотрудников, если их нет
    if (!context.Employees.Any())
    {
        // Получаем ID отделов
        var itDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "IT-отдел");
        var hrDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Отдел кадров");
        var salesDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Отдел продаж");
        var adminDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Администрация");
        var marketingDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Маркетинг");
        var accountingDepartment = await context.Departments.FirstOrDefaultAsync(d => d.Name == "Бухгалтерия");

        // Получаем ID должностей
        var programmer = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Программист");
        var hrManager = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Менеджер по персоналу");
        var salesManager = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Менеджер по продажам");
        var systemAdmin = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Системный администратор");
        var director = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Директор");
        var accountant = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Бухгалтер");
        var marketer = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Маркетолог");
        var analyst = await context.Positions.FirstOrDefaultAsync(p => p.Title == "Аналитик");

        var employees = new List<Employee>();

        // Сотрудник 1 - Программист в IT-отдел
        if (itDepartment != null && programmer != null)
        {
            employees.Add(new Employee
            {
                LastName = "Иванов",
                FirstName = "Иван",
                MiddleName = "Иванович",
                Email = "ivanov@example.com",
                PhoneNumber = "+7 (999) 123-45-67",
                BirthDate = new DateTime(1990, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2020, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Salary = 75000,
                DepartmentId = itDepartment.Id,
                PositionId = programmer.Id
            });
        }

        // Сотрудник 2 - Системный администратор в IT-отдел
        if (itDepartment != null && systemAdmin != null)
        {
            employees.Add(new Employee
            {
                LastName = "Петров",
                FirstName = "Петр",
                MiddleName = "Петрович",
                Email = "petrov@example.com",
                PhoneNumber = "+7 (999) 234-56-78",
                BirthDate = new DateTime(1988, 8, 20, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2019, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Salary = 85000,
                DepartmentId = itDepartment.Id,
                PositionId = systemAdmin.Id
            });
        }

        // Сотрудник 3 - Менеджер по персоналу в Отдел кадров
        if (hrDepartment != null && hrManager != null)
        {
            employees.Add(new Employee
            {
                LastName = "Сидорова",
                FirstName = "Анна",
                MiddleName = "Сергеевна",
                Email = "sidorova@example.com",
                PhoneNumber = "+7 (999) 345-67-89",
                BirthDate = new DateTime(1995, 12, 1, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2021, 6, 20, 0, 0, 0, DateTimeKind.Utc),
                Salary = 65000,
                DepartmentId = hrDepartment.Id,
                PositionId = hrManager.Id
            });
        }

        // Сотрудник 4 - Менеджер по продажам в Отдел продаж
        if (salesDepartment != null && salesManager != null)
        {
            employees.Add(new Employee
            {
                LastName = "Козлов",
                FirstName = "Дмитрий",
                MiddleName = "Алексеевич",
                Email = "kozlov@example.com",
                PhoneNumber = "+7 (999) 456-78-90",
                BirthDate = new DateTime(1992, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2020, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                Salary = 70000,
                DepartmentId = salesDepartment.Id,
                PositionId = salesManager.Id
            });
        }

        // Сотрудник 5 - Директор в Администрацию
        if (adminDepartment != null && director != null)
        {
            employees.Add(new Employee
            {
                LastName = "Морозов",
                FirstName = "Алексей",
                MiddleName = "Владимирович",
                Email = "director@example.com",
                PhoneNumber = "+7 (999) 567-89-01",
                BirthDate = new DateTime(1980, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2015, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Salary = 150000,
                DepartmentId = adminDepartment.Id,
                PositionId = director.Id
            });
        }

        // Сотрудник 6 - Бухгалтер в Бухгалтерию
        if (accountingDepartment != null && accountant != null)
        {
            employees.Add(new Employee
            {
                LastName = "Смирнова",
                FirstName = "Елена",
                MiddleName = "Александровна",
                Email = "smirnova@example.com",
                PhoneNumber = "+7 (999) 678-90-12",
                BirthDate = new DateTime(1985, 7, 25, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2018, 4, 10, 0, 0, 0, DateTimeKind.Utc),
                Salary = 80000,
                DepartmentId = accountingDepartment.Id,
                PositionId = accountant.Id
            });
        }

        // Сотрудник 7 - Маркетолог в Маркетинг
        if (marketingDepartment != null && marketer != null)
        {
            employees.Add(new Employee
            {
                LastName = "Волкова",
                FirstName = "Мария",
                MiddleName = "Игоревна",
                Email = "volkova@example.com",
                PhoneNumber = "+7 (999) 789-01-23",
                BirthDate = new DateTime(1993, 3, 8, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2021, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                Salary = 68000,
                DepartmentId = marketingDepartment.Id,
                PositionId = marketer.Id
            });
        }

        // Сотрудник 8 - Аналитик в Администрацию
        if (adminDepartment != null && analyst != null)
        {
            employees.Add(new Employee
            {
                LastName = "Тимофеев",
                FirstName = "Максим",
                MiddleName = "Андреевич",
                Email = "timofeev@example.com",
                PhoneNumber = "+7 (999) 890-12-34",
                BirthDate = new DateTime(1991, 11, 20, 0, 0, 0, DateTimeKind.Utc),
                HireDate = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc),
                Salary = 72000,
                DepartmentId = adminDepartment.Id,
                PositionId = analyst.Id
            });
        }

        if (employees.Any())
        {
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
            Console.WriteLine($"Добавлены тестовые сотрудники ({employees.Count} шт.)");
        }
    }
}

app.Run();