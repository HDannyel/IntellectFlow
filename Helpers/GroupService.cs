using IntellectFlow.DataModel;
using IntellectFlow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public class GroupService
    {
        private readonly IntellectFlowDbContext _context;

        public GroupService(IntellectFlowDbContext context)
        {
            _context = context;
        }

        public void AddStudentToGroup(int studentId, int groupId)
        {
            if (_context.StudentGroups.Any(sg => sg.StudentId == studentId && sg.GroupId == groupId))
                return; // Студент уже в группе

            var studentGroup = new StudentGroup
            {
                StudentId = studentId,
                GroupId = groupId
            };

            _context.StudentGroups.Add(studentGroup);
            _context.SaveChanges();
        }

       
    }
}
