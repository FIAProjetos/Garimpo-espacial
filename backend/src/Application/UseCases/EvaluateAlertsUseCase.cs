using Garimpo.Application.Ports;
using Garimpo.Domain.Entities.Alerts;

namespace Garimpo.Application.UseCases;

/// <summary>
/// Avalia aglomerados e telemetria, persistindo alertas de risco e integridade.
/// </summary>
public sealed class EvaluateAlertsUseCase
{
    private readonly IClusterRepository _clusterRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IAlertEvaluationService _alertEvaluation;
    private readonly IUnitOfWork _unitOfWork;

    public EvaluateAlertsUseCase(
        IClusterRepository clusterRepository,
        IAlertRepository alertRepository,
        IAlertEvaluationService alertEvaluation,
        IUnitOfWork unitOfWork)
    {
        _clusterRepository = clusterRepository;
        _alertRepository = alertRepository;
        _alertEvaluation = alertEvaluation;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> ExecuteAsync(
        double densityThreshold = 2.0,
        CancellationToken cancellationToken = default)
    {
        var clusters = await _clusterRepository.GetAllAsync(cancellationToken);
        var alerts = _alertEvaluation.EvaluateClusters(clusters, densityThreshold);

        await _alertRepository.ClearAsync(cancellationToken);

        if (alerts.Count > 0)
        {
            await _alertRepository.AddRangeAsync(alerts, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return alerts.Count;
    }

    public async Task RegisterTelemetryAlertAsync(
        string sensorId,
        int totalRecords,
        int rejectedRecords,
        CancellationToken cancellationToken = default)
    {
        Alert? alert = _alertEvaluation.EvaluateTelemetryIntegrity(sensorId, totalRecords, rejectedRecords);
        if (alert is null)
        {
            return;
        }

        await _alertRepository.AddRangeAsync([alert], cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
