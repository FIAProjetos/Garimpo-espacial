using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.Alerts;

/// <summary>
/// Alerta de integridade de telemetria: detecta anomalias na ingestao TLE que podem
/// indicar manipulacao de dados ou falha do sensor (vetor de ataque Red Team).
/// </summary>
public sealed class TelemetryIntegrityAlert : Alert
{
    public string SensorId { get; private set; } = string.Empty;
    public int RejectedRecords { get; private set; }
    public int TotalRecords { get; private set; }

    public override string GetAlertType() => "TelemetryIntegrity";

    private TelemetryIntegrityAlert()
    {
    }

    public static TelemetryIntegrityAlert Create(
        string sensorId,
        int rejectedRecords,
        int totalRecords)
    {
        double rejectionRate = totalRecords > 0 ? (double)rejectedRecords / totalRecords : 0;

        var severity = rejectionRate switch
        {
            >= 0.5 => AlertSeverity.Critical,
            >= 0.2 => AlertSeverity.Warning,
            > 0 => AlertSeverity.Info,
            _ => AlertSeverity.Info
        };

        return new TelemetryIntegrityAlert
        {
            Id = Guid.NewGuid(),
            SensorId = sensorId,
            RejectedRecords = rejectedRecords,
            TotalRecords = totalRecords,
            Severity = severity,
            TriggeredAt = DateTime.UtcNow
        };
    }

    public override string BuildMessage()
        => $"Sensor {SensorId}: {RejectedRecords}/{TotalRecords} registros TLE rejeitados "
         + "- possivel anomalia de integridade na telemetria.";

    public override bool RequiresImmediateAction()
        => Severity == AlertSeverity.Critical;
}
