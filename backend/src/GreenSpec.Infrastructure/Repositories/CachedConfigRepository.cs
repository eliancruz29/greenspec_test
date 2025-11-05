using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GreenSpec.Infrastructure.Repositories;

/// <summary>
/// Decorator that adds caching functionality to IConfigRepository
/// </summary>
public class CachedConfigRepositoryDecorator(
    IConfigRepository innerRepository,
    IMemoryCache cache) : IConfigRepository
{
    private const string ConfigCacheKey = "CurrentConfig";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public async Task<Config?> GetCurrentConfigAsync(CancellationToken cancellationToken = default)
    {
        // Try to get from cache first
        if (cache.TryGetValue<Config>(ConfigCacheKey, out var cachedConfig))
        {
            return cachedConfig;
        }

        // If not in cache, delegate to inner repository
        var config = await innerRepository.GetCurrentConfigAsync(cancellationToken);

        // Cache the result if found
        if (config != null)
        {
            cache.Set(ConfigCacheKey, config, CacheExpiration);
        }

        return config;
    }

    public async Task<Config> UpdateConfigAsync(Config config, CancellationToken cancellationToken = default)
    {
        // Delegate to inner repository
        var updatedConfig = await innerRepository.UpdateConfigAsync(config, cancellationToken);

        // Invalidate cache when config is updated
        cache.Remove(ConfigCacheKey);

        // Set the new config in cache
        cache.Set(ConfigCacheKey, updatedConfig, CacheExpiration);

        return updatedConfig;
    }
}
