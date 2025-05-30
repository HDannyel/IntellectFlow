    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace IntellectFlow.DataModel
    {
    public class Group : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
    }


}
