using Microsoft.AspNetCore.Identity;

namespace Курсач.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }

        public string? Department { get; set; }  // Добавил знак ? - поле стало необязательным

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
    }
}