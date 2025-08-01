using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmartWay.EFCore;
using SmartWay.Postgres.Models;

namespace SmartWay.EFCore
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Passport> Passports { get; set; }
        public DbSet<Department> Departments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5438;Database=WebAppEF;Username=admin;Password=adminadmin");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithOne(d => d.Employee)
                .HasForeignKey<Department>(d => d.EmployeeId);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Passport)
                .WithOne(p => p.Employee)
                .HasForeignKey<Passport>(d => d.EmployeeId);
        }


        //public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseNpgsql("Host=localhost;Port=5438;Database=WebAppEF;Username=admin;Password=adminadmin");
        //}

    }
}



