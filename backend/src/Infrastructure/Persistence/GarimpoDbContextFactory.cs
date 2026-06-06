using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Garimpo.Infrastructure.Persistence;

/// <summary>
/// Fabrica usada apenas em tempo de design pelas ferramentas do EF Core
/// (ex.: <c>dotnet ef migrations add</c>), sem precisar subir a aplicacao inteira.
/// A connection string pode vir da variavel de ambiente DEFAULT_CONNECTION.
/// </summary>
public sealed class GarimpoDbContextFactory : IDesignTimeDbContextFactory<GarimpoDbContext>
{
    public GarimpoDbContext CreateDbContext(string[] args)
    {
        string connectionString =
            Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=garimpo_db;Username=user;Password=password";

        var options = new DbContextOptionsBuilder<GarimpoDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new GarimpoDbContext(options);
    }
}
