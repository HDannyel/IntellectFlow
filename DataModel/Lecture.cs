using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Lecture : BaseEntity
    {
        public int Id { get; set; }
        public required string Title { get; set; }

        // Ссылка на документ
        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        // Связь с курсом
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
