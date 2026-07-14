using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(item => item.Id);
                entity.Property(item => item.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(item => item.LastName).IsRequired().HasMaxLength(100);
                entity.Property(item => item.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(item => item.Email).IsUnique();
                entity.Property(item => item.PhoneNumber).HasMaxLength(50);
                entity.Property(item => item.Department).IsRequired().HasMaxLength(100);
                entity.Property(item => item.Salary).HasColumnType("decimal(18,2)");
                entity.Property(item => item.HireDate).IsRequired();
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("AppUsers");
                entity.HasKey(item => item.Id);
                entity.Property(item => item.UserName).IsRequired().HasMaxLength(100);
                entity.HasIndex(item => item.UserName).IsUnique();
                entity.Property(item => item.Email).HasMaxLength(200);
                entity.Property(item => item.PasswordHash).HasMaxLength(500);
                entity.Property(item => item.AuthProvider).IsRequired().HasMaxLength(50);
            });
        }
    }
}
