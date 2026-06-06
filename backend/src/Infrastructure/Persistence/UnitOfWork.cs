using Garimpo.Application.Ports;

namespace Garimpo.Infrastructure.Persistence;

/// <summary>Adapter EF Core para <see cref="IUnitOfWork"/>.</summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly GarimpoDbContext _context;

    public UnitOfWork(GarimpoDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}
