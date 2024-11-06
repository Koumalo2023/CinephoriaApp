using CinephoriaServer.Models.PostgresqlDb;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.Configurations
{
    public class SeedAdmin
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Vérifiez si le rôle Admin existe, sinon, créez-le
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Vérifiez si l'administrateur existe déjà
            if (await userManager.FindByNameAsync("admin@cinephoria.com") == null)
            {
                // Créez l'administrateur
                var adminUser = new AppUser
                {
                    UserName = "admin@cinephoria.com",
                    Email = "admin@cinephoria.com",
                    FirstName = "Admin",
                    LastName = "Cinephoria",
                    Position = "Directeur",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    HiredDate = DateTime.UtcNow,
                    PhoneNumber = "062598631457",
                };

                // Créez l'utilisateur avec un mot de passe par défaut
                var result = await userManager.CreateAsync(adminUser, "AdminPassword-Cine-123");

                // Si la création réussit, assignez le rôle Admin
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }

}
