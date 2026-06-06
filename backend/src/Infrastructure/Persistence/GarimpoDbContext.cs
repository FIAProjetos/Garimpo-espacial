using Garimpo.Domain.Entities;
using Garimpo.Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;

namespace Garimpo.Infrastructure.Persistence;

/// <summary>
/// Contexto EF Core que mapeia o dominio do Garimpo Espacial para o PostgreSQL.
/// </summary>
public sealed class GarimpoDbContext : DbContext
{
    public GarimpoDbContext(DbContextOptions<GarimpoDbContext> options) : base(options)
    {
    }

    public DbSet<Debris> Debris => Set<Debris>();

    public DbSet<Cluster> Clusters => Set<Cluster>();

    public DbSet<Alert> Alerts => Set<Alert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GarimpoDbContext).Assembly);
    }
}
