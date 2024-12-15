using API_BackEnd.Migrations;
using Microsoft.AspNetCore.Identity;

namespace API_BackEnd.Data
{
    public class ApplicationUsers : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? Address { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}
