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
    }
}