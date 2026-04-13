using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Курсач.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        // Личные данные
        [Required(ErrorMessage = "Введите фамилию")]
        [MaxLength(50, ErrorMessage = "Фамилия не может быть длиннее 50 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [MaxLength(50, ErrorMessage = "Имя не может быть длиннее 50 символов")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "Отчество не может быть длиннее 50 символов")]
        [Display(Name = "Отчество")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Введите дату рождения")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        // Контактные данные
        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [MaxLength(20)]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        // Рабочая информация
        [DataType(DataType.Date)]
        [Display(Name = "Дата приема на работу")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Введите зарплату")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1000000, ErrorMessage = "Зарплата должна быть от 0 до 1 000 000")]
        [Display(Name = "Зарплата")]
        public decimal Salary { get; set; }

        // Внешние ключи
        [Required(ErrorMessage = "Выберите отдел")]
        [Display(Name = "Отдел")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Выберите должность")]
        [Display(Name = "Должность")]
        public int PositionId { get; set; }

        // Навигационные свойства
        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        [ForeignKey("PositionId")]
        public virtual Position? Position { get; set; }

        // === НОВЫЕ ПОЛЯ ===

        // Отпуск
        [DataType(DataType.Date)]
        [Display(Name = "Начало отпуска")]
        public DateTime? VacationStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Конец отпуска")]
        public DateTime? VacationEndDate { get; set; }

        // Квалификация и курсы
        public List<Qualification> Qualifications { get; set; } = new List<Qualification>();
    }

    // Новая модель для квалификации
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название курса")]
        public string CourseName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата начала курса")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Дата окончания курса")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Статус")]
        public QualificationStatus Status { get; set; } = QualificationStatus.Planned;

        [Display(Name = "Организация")]
        public string? Organization { get; set; }

        [Display(Name = "Сертификат/Удостоверение")]
        public string? CertificateNumber { get; set; }

        // Внешний ключ к сотруднику
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public enum QualificationStatus
    {
        [Display(Name = "Запланирован")]
        Planned,
        [Display(Name = "В процессе")]
        InProgress,
        [Display(Name = "Завершен")]
        Completed,
        [Display(Name = "Отменен")]
        Cancelled
    }
}