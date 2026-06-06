using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.SpaceAssets;

/// <summary>
/// Sensor de telemetria orbital (ex.: fonte TLE da Celestrak). Herda de <see cref="SpaceAsset"/>
/// e representa a infraestrutura de coleta de dados do ecossistema.
/// </summary>
public sealed class SensorAsset : SpaceAsset
{
    public string SensorId { get; private set; } = string.Empty;
    public string DataSource { get; private set; } = string.Empty;
    public DateTime? LastReadingAt { get; private set; }
    public bool IsOnline { get; private set; }

    public override AssetCategory Category => AssetCategory.Sensor;

    private SensorAsset()
    {
    }

    public static SensorAsset Create(string sensorId, string dataSource, bool isOnline = true)
    {
        return new SensorAsset
        {
            Id = Guid.NewGuid(),
            SensorId = sensorId,
            Name = $"Sensor-{sensorId}",
            DataSource = dataSource,
            IsOnline = isOnline,
            RegisteredAt = DateTime.UtcNow,
            LastReadingAt = isOnline ? DateTime.UtcNow : null
        };
    }

    public void RecordReading(DateTime timestamp)
    {
        LastReadingAt = timestamp;
        IsOnline = true;
    }

    public void MarkOffline()
    {
        IsOnline = false;
    }

    public override string GetSummary()
        => $"Sensor {SensorId} ({DataSource}) - {(IsOnline ? "online" : "offline")}";

    public override string GetTrackingId() => SensorId;
}
