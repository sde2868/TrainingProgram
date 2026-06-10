using Microsoft.EntityFrameworkCore;
using TraineeManagement.Models;

namespace TraineeManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
 
                modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
 
                modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
 
                // Seed Admin user
                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = 1,
                    UserName = "Admin_Zeus_Learning",
                    Email = "admin@zeuslearning.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = (Models.UserRole)UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                });
        }
    }
}