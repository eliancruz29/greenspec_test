using GreenSpec.Domain.Entities;

namespace GreenSpec.Domain.Interfaces;

public interface IConfigRepository
{
    Task<Config?> GetCurrentConfigAsync(CancellationToken cancellationToken = default);
    Task<Config> UpdateConfigAsync(Config config, CancellationToken cancellationToken = default);
}
