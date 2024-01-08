using Microsoft.AspNetCore.Identity;

namespace Agency.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
