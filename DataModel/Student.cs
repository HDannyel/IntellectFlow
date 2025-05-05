using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Student
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Связь с IdentityUser
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentTaskSubmission> TaskSubmissions { get; set; } = new List<StudentTaskSubmission>();
    }
}
