using GreenSpec.Domain.Entities;

namespace GreenSpec.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
