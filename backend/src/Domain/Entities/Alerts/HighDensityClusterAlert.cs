using Garimpo.Domain.Enums;

namespace Garimpo.Domain.Entities.Alerts;

/// <summary>
/// Alerta gerado quando um aglomerado de detritos ultrapassa limiar de densidade,
/// indicando zona de alto risco de colisao (Sindrome de Kessler).
/// </summary>
public sealed class HighDensityClusterAlert : Alert
{
    public Guid ClusterId { get; private set; }
    public int ClusterLabel { get; private set; }
    public double Density { get; private set; }
    public double CentroidAltitudeKm { get; private set; }
    public int MemberCount { get; private set; }

    public override string GetAlertType() => "HighDensityCluster";

    private HighDensityClusterAlert()
    {
    }

    public static HighDensityClusterAlert Create(Cluster cluster, double densityThreshold)
    {
        AlertSeverity severity = cluster.Density >= densityThreshold * 2
            ? AlertSeverity.Critical
            : cluster.Density >= densityThreshold
                ? AlertSeverity.Warning
                : AlertSeverity.Info;

        return new HighDensityClusterAlert
        {
            Id = Guid.NewGuid(),
            ClusterId = cluster.Id,
            ClusterLabel = cluster.Label,
            Density = cluster.Density,
            CentroidAltitudeKm = cluster.CentroidAltitudeKm,
            MemberCount = cluster.MemberCount,
            Severity = severity,
            TriggeredAt = DateTime.UtcNow
        };
    }

    public override string BuildMessage()
        => $"Aglomerado #{ClusterLabel} com densidade {Density:F2} em {CentroidAltitudeKm:F0} km "
         + $"({MemberCount} detritos) - risco de colisao em cadeia.";

    public override bool RequiresImmediateAction()
        => Severity >= AlertSeverity.Warning;
}
