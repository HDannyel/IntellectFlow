using IntellectFlow.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace IntellectFlow.Models
{
    public class IntellectFlowDbContext : DbContext
    {
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentTaskSubmission> StudentTaskSubmissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Discipline)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DisciplineId);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId);

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            modelBuilder.Entity<Assignment>()
                .HasMany(t => t.Submissions)
                .WithOne(s => s.Task)
                .HasForeignKey(s => s.TaskId);

            modelBuilder.Entity<StudentTaskSubmission>()
                .HasOne(s => s.Student)
                .WithMany(s => s.TaskSubmissions)
                .HasForeignKey(s => s.StudentId);

            // Уникальный индекс для InvitationCode
            modelBuilder.Entity<Course>()
                .HasIndex(c => c.InvitationCode)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["defaultConnectionString"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("defaultConnectionString in config file cannot be empty!");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
