using Microsoft.EntityFrameworkCore;
using PayrollApi.Models;

namespace PayrollApi.Data
{
    public class PayrollDbContext : DbContext
    {
        public PayrollDbContext(DbContextOptions<PayrollDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<EmployeeWorkingDay> EmployeeWorkingDays { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EmployeeNumber)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.DailyRate)
                      .HasColumnType("decimal(18,2)");

                entity.HasMany(e => e.WorkingDays)
                      .WithOne(d => d.Employee)
                      .HasForeignKey(d => d.EmployeeId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<EmployeeWorkingDay>(entity =>
            {
                entity.HasKey(e => e.Id);

                // DayNumber will store 1-7 mapped to Monday-Sunday
                entity.Property(e => e.DayNumber).IsRequired();

                entity.Property(e => e.DayName)
                      .IsRequired()
                      .HasMaxLength(10);
            });
        }
    }
}

