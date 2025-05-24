using IntellectFlow.DataModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Configuration;
using System.Linq;

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

        public DbSet<Group> Groups { get; set; }                // Добавлено
        public DbSet<StudentGroup> StudentGroups { get; set; }  // Добавлено
        public DbSet<User> Users { get; set; }                   // Добавлено

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка аудита для BaseEntity
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt")
                        .HasDefaultValueSql("GETUTCDATE()");

                    // modelBuilder.Entity(entityType.ClrType)
                    //     .Property<DateTime?>("UpdatedAt")
                    //     .HasComputedColumnSql("ISNULL([UpdatedAt], [CreatedAt])");
                }
            }

            // Конфигурация связей

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

            modelBuilder.Entity<StudentGroup>()                     // Конфигурация StudentGroup
                .HasKey(sg => new { sg.StudentId, sg.GroupId });

            modelBuilder.Entity<StudentGroup>()
                .HasOne(sg => sg.Student)
                .WithMany(s => s.StudentGroups)
                .HasForeignKey(sg => sg.StudentId);

            modelBuilder.Entity<StudentGroup>()
                .HasOne(sg => sg.Group)
                .WithMany(g => g.StudentGroups)
                .HasForeignKey(sg => sg.GroupId);

            modelBuilder.Entity<Assignment>()
                .HasMany(a => a.Submissions)
                .WithOne(s => s.Assignment)
                .HasForeignKey(s => s.AssignmentId);

            modelBuilder.Entity<StudentTaskSubmission>()
                .HasOne(s => s.Student)
                .WithMany(s => s.TaskSubmissions)
                .HasForeignKey(s => s.StudentId);

            modelBuilder.Entity<Lecture>()
                .HasOne(l => l.Document)
                .WithMany()
                .HasForeignKey(l => l.DocumentId)
                .OnDelete(DeleteBehavior.Cascade); // При удалении документа удаляется и лекция
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["defaultConnectionString"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Строка подключения 'defaultConnectionString' в App.config не найдена!");
            }
            optionsBuilder.UseSqlServer(connectionString);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChanges();
        }
    }
}
