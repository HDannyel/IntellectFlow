using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Assignment : BaseEntity
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public ICollection<StudentTaskSubmission> Submissions { get; set; } = new List<StudentTaskSubmission>();
    }
}
