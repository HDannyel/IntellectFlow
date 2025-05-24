using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntellectFlow.Helpers
{
    public class CourseService
    {
        private readonly IntellectFlowDbContext _context;

        public CourseService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public void AddStudentsFromGroupToCourse(int courseId, List<int> studentIds)
        {
            foreach (var studentId in studentIds)
            {
                if (!_context.StudentCourses.Any(sc => sc.CourseId == courseId && sc.StudentId == studentId))
                {
                    _context.StudentCourses.Add(new StudentCourse
                    {
                        CourseId = courseId,
                        StudentId = studentId,
                        EnrolledDate = DateTime.UtcNow
                    });
                }
            }

            _context.SaveChanges();
        }

      
    }
}
