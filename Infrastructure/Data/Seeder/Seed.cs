using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seeder;

public class Seed
{
    public static async Task SeedAdmin(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        if(!roleManager.RoleExistsAsync(Roles.Admin.ToString()).Result)
        {
            await roleManager.CreateAsync(new IdentityRole<int>(Roles.Admin.ToString()));
        }
        var user = userManager.Users.FirstOrDefault(x=> x.UserName == "Admin");
        if (user == null)
        {
            var admin = new User()
            {
                UserName = "Admin",
                FullName = "Admin",
                Email = "admin@gmail.com",
                CreatedDate = DateTime.UtcNow,
            };
            var res =  await userManager.CreateAsync(admin, "qwerty123");
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
    
    public static async Task<bool> SeedRole(RoleManager<IdentityRole<int>> roleManager)
    {
        var newRole = new List<IdentityRole<int>>()
        {
            new(Roles.Admin.ToString()),
            new(Roles.User.ToString()),
        };
        var roles = await roleManager.Roles.ToListAsync();
        foreach (var role in newRole)
        {
            if(roles.Any(r=>r.Name == role.Name))
                continue;
            await roleManager.CreateAsync(role);
        }
        return true;
    }
}