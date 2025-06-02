using Microsoft.AspNetCore.Identity;

namespace IntellectFlow.DataModel
{
    public class User : IdentityUser<int> // int — тип ключа
    {
        public string Name { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
    }
}
