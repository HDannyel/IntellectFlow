using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.DataModel
{
    public class StudentTaskSubmission : BaseEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;
        public required string SubmissionText { get; set; } // Ответ студента
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
        public int? AiScore { get; set; } // Оценка от нейросети
        public string? AiComment { get; set; } // Комментарий нейросети
        public bool IsAiChecked { get; set; } = false;
        public int? TeacherScore { get; set; } // Оценка от преподавателя
        public string? TeacherComment { get; set; } // Комментарий преподавателя
        public bool IsTeacherChecked { get; set; } = false;
        public SubmissionStatus Status { get; set; }
    }
    public enum SubmissionStatus
    {
        Pending,
        Submitted,
        Checked,
        Rejected
    }


}
