using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public interface IUserContext
    {
        int UserId { get; }
        string UserName { get; }
        string UserRole { get; }
        int? TeacherId { get; }
        int? StudentId { get; }

        void SetUser(int userId, string userName, string userRole, int? teacherId = null, int? studentId = null);
    }


}
