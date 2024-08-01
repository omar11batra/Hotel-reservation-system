using Hajz.Contants;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Hajz.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.مدير_النظام.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.مستخدم.ToString()));
              
            }
        }
    }
}
