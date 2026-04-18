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

    // Настройки пользователя - разрешаем русские буквы в логине
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

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

    // ... остальной код (отделы, должности, сотрудники) ...
}

app.Run();