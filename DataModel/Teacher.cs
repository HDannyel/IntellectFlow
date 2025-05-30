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

        public string Login { get; set; }       // добавлено
        public string Password { get; set; }    // надо захешировать

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}
