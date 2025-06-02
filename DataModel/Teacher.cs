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

        public string Name { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{Name} {MiddleName} {LastName}";

        // Внешний ключ на пользователя Identity
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}
