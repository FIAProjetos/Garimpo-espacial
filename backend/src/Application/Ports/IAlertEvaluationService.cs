using Garimpo.Domain.Entities;
using Garimpo.Domain.Entities.Alerts;

namespace Garimpo.Application.Ports;

/// <summary>Porta que abstrai o servico de avaliacao de alertas do dominio.</summary>
public interface IAlertEvaluationService
{
    IReadOnlyList<Alert> EvaluateClusters(IReadOnlyList<Cluster> clusters, double densityThreshold = 2.0);

    Alert? EvaluateTelemetryIntegrity(string sensorId, int totalRecords, int rejectedRecords);
}
