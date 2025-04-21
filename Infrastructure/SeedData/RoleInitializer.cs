using Microsoft.AspNetCore.Identity;

namespace Infrastructure.SeedData
{
    public static class RoleInitializer
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Trainee", "Coach" };

            /*foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }*/
        }
    }
}
