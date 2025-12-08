using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace RazorPagesMovie.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            // Run async code synchronously
            Task.Run(async () =>
            {
                // 1. Create roles
                string[] roles = { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                // 2. Create default admin
                var adminEmail = "admin@ngwako.com";
                var adminPassword = "Admin@123";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(adminUser, adminPassword);
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // 3. Create default normal user (optional)
                var userEmail = "user@example.com";
                var userPassword = "User@123";
                var normalUser = await userManager.FindByEmailAsync(userEmail);
                if (normalUser == null)
                {
                    normalUser = new IdentityUser { UserName = userEmail, Email = userEmail, EmailConfirmed = true };
                    await userManager.CreateAsync(normalUser, userPassword);
                    await userManager.AddToRoleAsync(normalUser, "User");
                }

            }).GetAwaiter().GetResult();
        }
    }
}
