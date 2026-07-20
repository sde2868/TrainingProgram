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
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTask> LearningTasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<TaskSubmission> TaskSubmissions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Mentor>().HasIndex(m => m.Email).IsUnique();

            modelBuilder.Entity<ProcessingJob>().HasIndex(x => x.MessageId).IsUnique();

            modelBuilder.Entity<ProcessingJob>().HasIndex(x => x.CorrelationId);

            modelBuilder.Entity<ProcessingJob>().HasIndex(x => x.Status);

            // Seed Admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                // Id = 1,
                UserName = "Admin_Zeus_Learning",
                Email = "admin@zeuslearning.com",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = (Models.UserRole)UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            });

            // TaskAssignment relationships
            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Trainee)
                .WithMany()
                .HasForeignKey(t => t.TraineeId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.Mentor)
                .WithMany()
                .HasForeignKey(t => t.MentorId);

            modelBuilder.Entity<TaskAssignment>()
                .HasOne(t => t.LearningTask)
                .WithMany()
                .HasForeignKey(t => t.LearningTaskId);


            // TaskSubmission -> TaskAssignment
            modelBuilder.Entity<TaskSubmission>()
                .HasOne(s => s.TaskAssignment)
                .WithMany(t => t.Submissions)
                .HasForeignKey(s => s.TaskAssignmentId);

            modelBuilder.Entity<SubmissionFile>()
                .HasOne(f => f.Submission)
                .WithMany(s => s.Files)
                .HasForeignKey(f => f.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review -> TaskSubmission
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Submission)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.TaskSubmissionId);

            // Review -> Mentor
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Mentor)
                .WithMany()
                .HasForeignKey(r => r.MentorId);

            // ProcessingJob -> TaskSubmission
            modelBuilder.Entity<ProcessingJob>()
                .HasOne(x => x.Submission)
                .WithMany()
                .HasForeignKey(x => x.SubmissionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ProcessingJob -> SubmissionFIle
            modelBuilder.Entity<ProcessingJob>()
                .HasOne(x => x.SubmissionFile)
                .WithMany()
                .HasForeignKey(x => x.SubmissionFileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}