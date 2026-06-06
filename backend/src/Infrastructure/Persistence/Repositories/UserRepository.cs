using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garimpo.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly GarimpoDbContext _context;

    public UserRepository(GarimpoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalized = User.NormalizeEmail(email);

        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalized = User.NormalizeEmail(email);

        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == normalized, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
}
