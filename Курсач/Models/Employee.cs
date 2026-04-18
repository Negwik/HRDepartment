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

        [Required(ErrorMessage = "Введите номер телефона")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [MaxLength(20)]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        // Адрес и гражданство
        [Required(ErrorMessage = "Введите адрес проживания")]
        [MaxLength(300)]
        [Display(Name = "Адрес проживания")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Введите гражданство")]
        [MaxLength(100)]
        [Display(Name = "Гражданство")]
        public string Citizenship { get; set; }

        // Рабочая информация
        [Required(ErrorMessage = "Введите дату приема на работу")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата приема на работу")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Введите зарплату")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000, ErrorMessage = "Зарплата должна быть от 0 до 10 000 000")]
        [Display(Name = "Зарплата")]
        public decimal Salary { get; set; } = 0;

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

        //ОТПУСК
        [Display(Name = "Тип отпуска")]
        public VacationType VacationType { get; set; } = VacationType.Annual;

        [DataType(DataType.Date)]
        [Display(Name = "Начало отпуска")]
        public DateTime? VacationStartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Конец отпуска")]
        public DateTime? VacationEndDate { get; set; }

        //КВАЛИФИКАЦИЯ
        public List<Qualification> Qualifications { get; set; } = new List<Qualification>();

        //ОБРАЗОВАНИЕ
        [Required(ErrorMessage = "Выберите образование")]
        [Display(Name = "Образование")]
        public EducationLevel Education { get; set; }

        [Required(ErrorMessage = "Введите специальность")]
        [MaxLength(200)]
        [Display(Name = "Специальность по диплому")]
        public string Specialty { get; set; }

        [Required(ErrorMessage = "Введите учебное заведение")]
        [MaxLength(100)]
        [Display(Name = "Учебное заведение")]
        public string EducationalInstitution { get; set; }

        [Required(ErrorMessage = "Введите год окончания")]
        [DataType(DataType.Date)]
        [Display(Name = "Год окончания")]
        public DateTime? GraduationYear { get; set; }

        // ИНВАЛИДНОСТЬ
        [Display(Name = "Инвалидность")]
        public DisabilityStatus Disability { get; set; } = DisabilityStatus.None;

        [MaxLength(200)]
        [Display(Name = "Группа инвалидности (если есть)")]
        public string? DisabilityGroup { get; set; }

        // СЕМЕЙНОЕ ПОЛОЖЕНИЕ
        [Required(ErrorMessage = "Выберите семейное положение")]
        [Display(Name = "Семейное положение")]
        public MaritalStatus MaritalStatus { get; set; }

        [Display(Name = "Количество детей")]
        public int ChildrenCount { get; set; } = 0;

        [Display(Name = "Информация о детях")]
        public string? ChildrenInfo { get; set; }
    }

    // Тип отпуска
    public enum VacationType
    {
        [Display(Name = "Ежегодный оплачиваемый отпуск")]
        Annual,
        [Display(Name = "Административный отпуск (без сохранения зарплаты)")]
        Administrative,
        [Display(Name = "Декретный отпуск")]
        ParentalLeave,
        [Display(Name = "Больничный")]
        SickLeave
    }

    // Образование
    public enum EducationLevel
    {
        [Display(Name = "Среднее")]
        Secondary,
        [Display(Name = "Среднее специальное")]
        SpecializedSecondary,
        [Display(Name = "Неоконченное высшее")]
        IncompleteHigher,
        [Display(Name = "Высшее")]
        Higher,
        [Display(Name = "Два и более высших")]
        TwoOrMoreHigher,
        [Display(Name = "Ученая степень")]
        AcademicDegree
    }

    // Инвалидность
    public enum DisabilityStatus
    {
        [Display(Name = "Нет инвалидности")]
        None,
        [Display(Name = "Инвалид 1 группы")]
        Group1,
        [Display(Name = "Инвалид 2 группы")]
        Group2,
        [Display(Name = "Инвалид 3 группы")]
        Group3,
        [Display(Name = "Ребенок-инвалид")]
        ChildDisabled
    }

    // Семейное положение
    public enum MaritalStatus
    {
        [Display(Name = "Не замужем / Не женат")]
        Single,
        [Display(Name = "Замужем / Женат")]
        Married,
        [Display(Name = "Разведен(а)")]
        Divorced,
        [Display(Name = "Вдовец / Вдова")]
        Widowed,
        [Display(Name = "Гражданский брак")]
        CivilMarriage
    }
}