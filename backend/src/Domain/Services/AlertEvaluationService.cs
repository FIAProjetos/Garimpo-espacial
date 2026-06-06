using Garimpo.Domain.Entities;
using Garimpo.Domain.Entities.Alerts;

namespace Garimpo.Domain.Services;

/// <summary>
/// Servico de dominio que avalia aglomerados e telemetria, gerando alertas polimorficos
/// conforme limiares de densidade e integridade de dados.
/// </summary>
public sealed class AlertEvaluationService
{
    private const double DefaultDensityThreshold = 2.0;
    private const double TelemetryRejectionThreshold = 0.1;

    /// <summary>Avalia aglomerados e retorna alertas de alta densidade.</summary>
    public IReadOnlyList<Alert> EvaluateClusters(
        IReadOnlyList<Cluster> clusters,
        double densityThreshold = DefaultDensityThreshold)
    {
        return clusters
            .Where(c => c.Density >= densityThreshold)
            .Select(c => (Alert)HighDensityClusterAlert.Create(c, densityThreshold))
            .ToList();
    }

    /// <summary>Avalia integridade da telemetria ingerida e gera alerta se necessario.</summary>
    public Alert? EvaluateTelemetryIntegrity(
        string sensorId,
        int totalRecords,
        int rejectedRecords)
    {
        if (totalRecords == 0)
        {
            return null;
        }

        double rejectionRate = (double)rejectedRecords / totalRecords;
        if (rejectionRate < TelemetryRejectionThreshold)
        {
            return null;
        }

        return TelemetryIntegrityAlert.Create(sensorId, rejectedRecords, totalRecords);
    }
}
