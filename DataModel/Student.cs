using IntellectFlow.DataModel;


namespace IntellectFlow.DataModel
{
    public class Student : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{Name} {MiddleName} {LastName}";

        public int? UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<StudentTaskSubmission> TaskSubmissions { get; set; } = new List<StudentTaskSubmission>();
        public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
    }
}