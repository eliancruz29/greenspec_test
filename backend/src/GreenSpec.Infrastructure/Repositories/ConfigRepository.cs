using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GreenSpec.Infrastructure.Repositories;

public class ConfigRepository(ApplicationDbContext context) : IConfigRepository
{
    public async Task<Config?> GetCurrentConfigAsync(CancellationToken cancellationToken = default)
    {
        return await context.Configs.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Config> UpdateConfigAsync(Config config, CancellationToken cancellationToken = default)
    {
        context.Configs.Update(config);
        await context.SaveChangesAsync(cancellationToken);
        return config;
    }
}
