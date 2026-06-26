using GalvaERP.Domain.Entities;
using GalvaERP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GalvaERP.Common.Security;

public static class AdminUserSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("AdminUserSeeder");

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Master_Users.AnyAsync())
            {
                return;
            }

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var admin = new Master_User
            {
                Username = "admin",
                PasswordHash = passwordHasher.HashPassword("admin123"),
                Role = "Admin",
            };

            context.Master_Users.Add(admin);
            await context.SaveChangesAsync();

            logger.LogInformation("Seeded admin user (username: admin, default password: admin123). Change it immediately.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed admin user.");
        }
    }
}
