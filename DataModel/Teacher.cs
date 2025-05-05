using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IntellectFlow.DataModel
{
    public class Teacher
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Связь с IdentityUser
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
