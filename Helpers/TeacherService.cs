using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class TeacherService
    {
        private readonly IntellectFlowDbContext _context;

        public TeacherService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetCoursesByTeacherAsync(int teacherId)
        {
            return await _context.Courses
                .Include(c => c.Discipline)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }

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
        public async Task DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteDisciplineAsync(int disciplineId)
        {
            var discipline = await _context.Disciplines.FindAsync(disciplineId);
            if (discipline != null)
            {
                _context.Disciplines.Remove(discipline);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveStudentFromCourseAsync(int studentId, int courseId)
        {
            var studentCourse = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (studentCourse != null)
            {
                _context.StudentCourses.Remove(studentCourse);
                await _context.SaveChangesAsync();
            }
        }

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

        public async Task<List<Student>> GetStudentsNotInCourseAsync(int courseId)
        {
            return await _context.Students
                .Where(s => !_context.StudentCourses
                    .Any(sc => sc.StudentId == s.Id && sc.CourseId == courseId))
                .ToListAsync();
        }
    }
}
