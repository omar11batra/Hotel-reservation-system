using Hajz.Contants;
using Hajz.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Hajz.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsyac(UserManager<User> userManager)
        {
            var defaultUser = new User
            {
                UserName="Admin",
                IsActive=true,
            };

            var user= await userManager.FindByNameAsync(defaultUser.UserName);
            if(user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin");
                await userManager.AddToRoleAsync(defaultUser,Roles.مدير_النظام.ToString());
            }
        }
    }
}
