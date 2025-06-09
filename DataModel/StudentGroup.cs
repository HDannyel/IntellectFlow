    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace IntellectFlow.DataModel
    {
        public class StudentGroup : BaseEntity
        {
            public int StudentId { get; set; }
            public Student Student { get; set; } = null!;

            public int GroupId { get; set; }
            public Group Group { get; set; } = null!;
        }

    }
