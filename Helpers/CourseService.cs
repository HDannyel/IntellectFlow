using IntellectFlow.DataModel;
using IntellectFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace IntellectFlow.Helpers
{
    public class CourseService
    {
        private readonly IntellectFlowDbContext _context;

        public CourseService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public async Task<Course> CreateCourse(string name, string description, int disciplineId, int teacherId)
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

        public async Task<IEnumerable<Course>> GetCoursesByTeacher(int teacherId)
        {
            return await _context.Courses
                .Include(c => c.Discipline)
                .Where(c => c.TeacherId == teacherId)
                .ToListAsync();
        }
    }
}