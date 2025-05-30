using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace IntellectFlow.Helpers
{
    public class TeacherService
    {
        private readonly IntellectFlowDbContext _context;

        public TeacherService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить все курсы преподавателя
        /// </summary>
        public async Task<List<Course>> GetCoursesByTeacherAsync(int teacherId)
        {
            return await _context.Courses
                .Include(c => c.Discipline)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

        /// <summary>
        /// Создать новый курс
        /// </summary>
        public async Task<Course> CreateCourseAsync(string name, string description, int disciplineId, int teacherId)
        {
            var course = new Course
            {
                Name = name,
                Description = description,
                DisciplineId = disciplineId,
                TeacherId = teacherId
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return course;
        }

        /// <summary>
        /// Добавить студента в курс
        /// </summary>
        public async Task AddStudentToCourseAsync(int studentId, int courseId)
        {
            var exists = await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (exists) return;

            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrolledDate = DateTime.UtcNow
            };

            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Получить студентов, не добавленных в курс
        /// </summary>
        public async Task<List<Student>> GetStudentsNotInCourseAsync(int courseId)
        {
            return await _context.Students
                .Where(s => !_context.StudentCourses
                    .Any(sc => sc.StudentId == s.Id && sc.CourseId == courseId))
                .ToListAsync();
        }
    }
}
