using Microsoft.AspNetCore.Identity;

namespace Hajz.Models
{
    public class User:IdentityUser
    {
        public bool IsActive { get; set; }
    }
}
