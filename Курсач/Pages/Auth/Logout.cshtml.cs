using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Курсач.Models;

namespace Курсач.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Выход из аккаунта
            await _signInManager.SignOutAsync();

            // Перенаправляем на страницу входа
            return RedirectToPage("/Auth/Login");
        }

        // Также обрабатываем GET запрос (если кто-то перейдет по прямой ссылке)
        public async Task<IActionResult> OnGetAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Auth/Login");
        }
    }
}