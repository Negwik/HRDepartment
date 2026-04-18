using System.ComponentModel.DataAnnotations;

namespace Курсач.Models
{
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