using Microsoft.AspNetCore.Identity;
using Ntech.WebApp.Constants;
using System.Threading.Tasks;

namespace Ntech.WebApp.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRoleAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Manager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Customer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Staff.ToString()));
        }
    }
}