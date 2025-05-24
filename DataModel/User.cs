using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public enum UserRole
    {
        Administrator,
        Teacher,
        Student
    }

    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public bool IsAdmin { get; set; }


        // Связь с сущностями
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }


}
