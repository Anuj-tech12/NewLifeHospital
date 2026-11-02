using Microsoft.AspNetCore.Identity;

namespace NewLifeHospital.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();


        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string roleName = "Admin";

            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(
                    new IdentityRole(roleName));
            }

            string? email = configuration["AdminAccount:Email"];
            string? password = configuration["AdminAccount:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "AdminAccount Email and Password are missing from configuration.");
            }

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    admin, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, roleName))
            {
                await userManager.AddToRoleAsync(admin, roleName);
            }
        }
    }


}
