using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IntellectFlow.DataModel
{
    public class Discipline : BaseEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

    }
}
