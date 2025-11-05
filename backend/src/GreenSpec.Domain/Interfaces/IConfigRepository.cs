using GreenSpec.Domain.Entities;

namespace GreenSpec.Domain.Interfaces;

public interface IConfigRepository
{
    Task<Config?> GetCurrentConfigAsync(CancellationToken cancellationToken = default);
    Task<Config> CreateNewConfigVersionAsync(Config config, CancellationToken cancellationToken = default);
    Task DeactivateAllConfigsAsync(CancellationToken cancellationToken = default);
}
