namespace Garimpo.Domain.Ports;

/// <summary>
/// Porta de dominio para sensores de telemetria orbital. A implementacao concreta
/// (ex.: Celestrak TLE) fica na infraestrutura, mantendo o nucleo desacoplado.
/// </summary>
public interface ISensor
{
    string SensorId { get; }
    string DataSource { get; }
    bool IsOnline { get; }

    /// <summary>Captura leituras brutas do sensor (ex.: bloco TLE).</summary>
    Task<SensorCaptureResult> CaptureAsync(CancellationToken cancellationToken = default);
}

/// <summary>Resultado de uma captura de sensor com metricas de integridade.</summary>
public sealed record SensorCaptureResult(
    string RawPayload,
    int TotalRecords,
    int ValidRecords,
    int RejectedRecords,
    DateTime CapturedAt);
