using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IntellectFlow.DataModel
{
    public class Teacher : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{Name} {MiddleName} {LastName}";

        // Связь с пользователем Identity
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}
