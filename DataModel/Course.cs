using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        //public required string? InvitationCode { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; } = null!;
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
