using Garimpo.Domain.Ports;
using Garimpo.Infrastructure.ExternalServices;
using Garimpo.Infrastructure.Tle;
using Microsoft.Extensions.Logging;

namespace Garimpo.Infrastructure.Adapters;

/// <summary>
/// Adapter de sensor que encapsula a captura de telemetria TLE da Celestrak,
/// expondo metricas de integridade para avaliacao de alertas.
/// </summary>
public sealed class TleSensorAdapter : ISensor
{
    public const string DefaultSensorId = "celestrak-tle";

    private readonly HttpClient _httpClient;
    private readonly TleParser _parser;
    private readonly ILogger<TleSensorAdapter> _logger;

    public string SensorId => DefaultSensorId;
    public string DataSource => "Celestrak/NORAD";
    public bool IsOnline { get; private set; } = true;

    public TleSensorAdapter(HttpClient httpClient, TleParser parser, ILogger<TleSensorAdapter> logger)
    {
        _httpClient = httpClient;
        _parser = parser;
        _logger = logger;
    }

    public async Task<SensorCaptureResult> CaptureAsync(CancellationToken cancellationToken = default)
    {
        const string group = "cosmos-2251-debris";
        string requestUri = $"gp.php?GROUP={Uri.EscapeDataString(group)}&FORMAT=tle";

        _logger.LogInformation("Sensor {SensorId} capturando telemetria de {Source}.", SensorId, DataSource);

        using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync(cancellationToken);
        var parsed = _parser.Parse(content, DateTime.UtcNow);

        int totalLines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;
        int validRecords = parsed.Count;
        int rejected = Math.Max(0, (totalLines / 3) - validRecords);

        IsOnline = true;

        return new SensorCaptureResult(content, totalLines, validRecords, rejected, DateTime.UtcNow);
    }
}
