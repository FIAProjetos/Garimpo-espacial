using Garimpo.Application.Ports;
using Garimpo.Domain.Entities;
using Garimpo.Domain.Entities.Alerts;
using Garimpo.Domain.Services;

namespace Garimpo.Infrastructure.Adapters;

public sealed class AlertEvaluationAdapter : IAlertEvaluationService
{
    private readonly AlertEvaluationService _service = new();

    public IReadOnlyList<Alert> EvaluateClusters(IReadOnlyList<Cluster> clusters, double densityThreshold = 2.0)
        => _service.EvaluateClusters(clusters, densityThreshold);

    public Alert? EvaluateTelemetryIntegrity(string sensorId, int totalRecords, int rejectedRecords)
        => _service.EvaluateTelemetryIntegrity(sensorId, totalRecords, rejectedRecords);
}
