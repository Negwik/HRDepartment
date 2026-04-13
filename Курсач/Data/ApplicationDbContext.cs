using Курсач.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Курсач.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Qualification> Qualifications { get; set; } // Добавляем

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Связь Qualification с Employee
            builder.Entity<Qualification>()
                .HasOne(q => q.Employee)
                .WithMany(e => e.Qualifications)
                .HasForeignKey(q => q.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}