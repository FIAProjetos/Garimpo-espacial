using Garimpo.Application.Ports;
using Garimpo.Infrastructure.Adapters;
using Garimpo.Infrastructure.ExternalServices;
using Garimpo.Infrastructure.Persistence;
using Garimpo.Infrastructure.Persistence.Repositories;
using Garimpo.Infrastructure.Persistence.Seeding;
using Garimpo.Infrastructure.Security;
using Garimpo.Infrastructure.Tle;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Garimpo.Infrastructure;

/// <summary>
/// Registro dos adapters de infraestrutura (persistencia, fonte externa e clustering).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection nao configurada. "
                + "Defina no arquivo .env (copie de .env.example) ou nas variaveis de ambiente.");
        }

        string celestrakBaseUrl = configuration["ExternalServices:Celestrak:BaseUrl"]
            ?? "https://celestrak.org/NORAD/elements/";

        services.AddDbContext<GarimpoDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IDebrisRepository, DebrisRepository>();
        services.AddScoped<IClusterRepository, ClusterRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton<IClusteringService, DbscanClusteringAdapter>();
        services.AddSingleton<IAlertEvaluationService, AlertEvaluationAdapter>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddScoped<DatabaseSeeder>();

        services.AddSingleton<TleParser>();
        services.AddHttpClient<ITleProvider, CelestrakTleProvider>(client =>
        {
            client.BaseAddress = new Uri(celestrakBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("User-Agent", "GarimpoEspacial/1.0");
        });

        return services;
    }
}
