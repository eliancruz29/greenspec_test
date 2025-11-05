using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GreenSpec.Infrastructure.Repositories;

public class ConfigRepository(ApplicationDbContext context) : IConfigRepository
{
    public async Task<Config?> GetCurrentConfigAsync(CancellationToken cancellationToken = default)
    {
        return await context.Configs
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Config> CreateNewConfigVersionAsync(Config config, CancellationToken cancellationToken = default)
    {
        await context.Configs.AddAsync(config, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return config;
    }

    public async Task DeactivateAllConfigsAsync(CancellationToken cancellationToken = default)
    {
        await context.Configs
            .Where(c => c.IsActive)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(c => c.IsActive, false)
                    .SetProperty(c => c.UpdatedAt, DateTime.UtcNow),
                cancellationToken);
    }
}
