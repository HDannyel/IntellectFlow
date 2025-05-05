using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class Lecture
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; } // Текст или URL файла
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
