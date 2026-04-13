using Курсач.Models;
using System.ComponentModel.DataAnnotations;

namespace Курсач.Models
{
    public class Position
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название должности")]
        [MaxLength(100, ErrorMessage = "Название не может быть длиннее 100 символов")]
        [Display(Name = "Название должности")]
        public string Title { get; set; }

        // Связь с сотрудниками (один ко многим)
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}