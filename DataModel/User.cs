using Microsoft.AspNetCore.Identity;

namespace IntellectFlow.DataModel
{
    public class User : IdentityUser<int> // Используем int как тип ID
    {
        public string Name { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
    }
}