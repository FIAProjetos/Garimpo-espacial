using Garimpo.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Garimpo.Application;

/// <summary>
/// Registro dos casos de uso da camada de aplicacao no container de injecao de dependencia.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IngestTleUseCase>();
        services.AddScoped<RunClusteringUseCase>();
        services.AddScoped<GetClustersUseCase>();
        services.AddScoped<GetDebrisUseCase>();
        services.AddScoped<GetAlertsUseCase>();
        services.AddScoped<EvaluateAlertsUseCase>();
        services.AddScoped<AcknowledgeAlertUseCase>();
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();

        return services;
    }
}
