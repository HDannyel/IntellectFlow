using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class UserContext : IUserContext
    {
        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserRole { get; private set; }
        public int? TeacherId { get; private set; }
        public int? StudentId { get; private set; }  // Добавлено

        public void SetUser(int userId, string userName, string userRole, int? teacherId = null, int? studentId = null)
        {
            UserId = userId;
            UserName = userName;
            UserRole = userRole;
            TeacherId = teacherId;
            StudentId = studentId;  // Инициализация
        }
    }



}
