using GreenSpec.Application.Interfaces;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.BackgroundServices;
using GreenSpec.Infrastructure.Persistence;
using GreenSpec.Infrastructure.Repositories;
using GreenSpec.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GreenSpec.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Memory Cache
        services.AddMemoryCache();

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );

        // Repositories
        // Register ConfigRepository as the base implementation
        services.AddScoped<ConfigRepository>();
        // Register IConfigRepository with the decorated (cached) implementation
        services.AddScoped<IConfigRepository>(provider =>
        {
            var innerRepository = provider.GetRequiredService<ConfigRepository>();
            var cache = provider.GetRequiredService<IMemoryCache>();
            return new CachedConfigRepositoryDecorator(innerRepository, cache);
        });
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAlertNotifier, AlertNotifier>();

        // Background Services
        services.AddHostedService<SensorSimulationService>();

        return services;
    }
}
