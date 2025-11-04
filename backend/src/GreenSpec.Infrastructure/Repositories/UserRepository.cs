using GreenSpec.Domain.Entities;
using GreenSpec.Domain.Interfaces;
using GreenSpec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GreenSpec.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }
}
