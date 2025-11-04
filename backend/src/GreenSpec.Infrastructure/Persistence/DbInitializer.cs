using GreenSpec.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GreenSpec.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed data if database is empty
        await SeedDataAsync(context);
    }

    private static async Task SeedDataAsync(ApplicationDbContext context)
    {
        // Seed default configuration if not exists
        if (!await context.Configs.AnyAsync())
        {
            var config = Config.Create(tempMax: 30m, humidityMax: 80m);
            context.Configs.Add(config);
            await context.SaveChangesAsync();
        }

        // Seed demo user if not exists
        if (!await context.Users.AnyAsync())
        {
            // Password: admin123
            // BCrypt hash for "admin123"
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
            var user = User.Create("admin", passwordHash);
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}
