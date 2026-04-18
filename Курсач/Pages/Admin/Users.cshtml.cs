using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Курсач.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Курсач.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersModel(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<User> Users { get; set; } = new();
        public Dictionary<string, string> UserRoles { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            Users = await _userManager.Users.ToListAsync();
            UserRoles.Clear();

            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UserRoles[user.Id] = roles.FirstOrDefault() ?? "User";
            }
        }

        // Создание пользователя
        public async Task<IActionResult> OnPostCreateUserAsync(string userName, string email, string password, string confirmPassword, string fullName, string role)
        {
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Пароли не совпадают";
                await LoadUsers();
                return Page();
            }

            var existingUser = await _userManager.FindByNameAsync(userName);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Пользователь с таким логином уже существует";
                await LoadUsers();
                return Page();
            }

            existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "Пользователь с таким email уже существует";
                await LoadUsers();
                return Page();
            }

            var newUser = new User
            {
                UserName = userName,
                Email = email,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(role))
                {
                    await _userManager.AddToRoleAsync(newUser, role);
                }
                TempData["SuccessMessage"] = $"Пользователь {userName} успешно создан";
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Ошибка создания: {errors}";
            }

            await LoadUsers();
            return Page();
        }

        // Редактирование пользователя
        public async Task<IActionResult> OnPostEditUserAsync(string userId, string userName, string email, string fullName, string role, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден";
                await LoadUsers();
                return Page();
            }

            // Обновляем данные
            user.UserName = userName;
            user.Email = email;
            user.FullName = fullName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                TempData["ErrorMessage"] = $"Ошибка обновления: {errors}";
                await LoadUsers();
                return Page();
            }

            // Обновляем пароль, если указан
            if (!string.IsNullOrEmpty(newPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!passwordResult.Succeeded)
                {
                    var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Ошибка смены пароля: {errors}";
                    await LoadUsers();
                    return Page();
                }
            }

            // Обновляем роль
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);

            TempData["SuccessMessage"] = $"Пользователь {userName} успешно обновлен";
            await LoadUsers();
            return Page();
        }

        // Удаление пользователя
        public async Task<IActionResult> OnGetDeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (user.UserName == User.Identity.Name)
                {
                    TempData["ErrorMessage"] = "Нельзя удалить собственную учетную запись";
                }
                else
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = $"Пользователь {user.UserName} удален";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ошибка при удалении";
                    }
                }
            }

            await LoadUsers();
            return RedirectToPage();
        }
    }
}