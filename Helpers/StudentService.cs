using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.Helpers
{
    public class StudentService
    {
        private readonly IntellectFlowDbContext _context;

        public StudentService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddStudentToCourse(int studentId, int courseId)
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

        public async Task<IEnumerable<Student>> GetStudentsNotInCourse(int courseId)
        {
            return await _context.Students
                .Where(s => !_context.StudentCourses
                    .Any(sc => sc.StudentId == s.Id && sc.CourseId == courseId))
                .ToListAsync();
        }
    }
}