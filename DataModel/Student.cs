﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Student : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MidleName { get; set; }
        public string LastName { get; set; }

        public string Login { get; set; }       // добавлено
        public string Password { get; set; }    // добавлено

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentTaskSubmission> TaskSubmissions { get; set; } = new List<StudentTaskSubmission>();
        public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
    }

}
