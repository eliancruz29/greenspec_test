using GreenSpec.Application.Interfaces;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.BackgroundServices;
using GreenSpec.Infrastructure.Persistence;
using GreenSpec.Infrastructure.Repositories;
using GreenSpec.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GreenSpec.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            )
        );

        // Repositories
        services.AddScoped<IConfigRepository, ConfigRepository>();
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
