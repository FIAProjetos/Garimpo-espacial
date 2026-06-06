using Garimpo.Domain.Entities.Alerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garimpo.Infrastructure.Persistence.Configurations;

public sealed class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts");

        builder.HasKey(a => a.Id);

        builder.HasDiscriminator<string>("discriminator")
            .HasValue<HighDensityClusterAlert>("HighDensityCluster")
            .HasValue<TelemetryIntegrityAlert>("TelemetryIntegrity");

        builder.Property(a => a.Severity).HasConversion<string>().HasMaxLength(16);
        builder.Property(a => a.TriggeredAt);
        builder.Property(a => a.IsAcknowledged);
        builder.Property(a => a.AcknowledgedAt);

        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => a.TriggeredAt);
    }
}

public sealed class HighDensityClusterAlertConfiguration : IEntityTypeConfiguration<HighDensityClusterAlert>
{
    public void Configure(EntityTypeBuilder<HighDensityClusterAlert> builder)
    {
        builder.Property(a => a.ClusterId);
        builder.Property(a => a.ClusterLabel);
        builder.Property(a => a.Density);
        builder.Property(a => a.CentroidAltitudeKm);
        builder.Property(a => a.MemberCount);
    }
}

public sealed class TelemetryIntegrityAlertConfiguration : IEntityTypeConfiguration<TelemetryIntegrityAlert>
{
    public void Configure(EntityTypeBuilder<TelemetryIntegrityAlert> builder)
    {
        builder.Property(a => a.SensorId).HasMaxLength(64);
        builder.Property(a => a.RejectedRecords);
        builder.Property(a => a.TotalRecords);
    }
}
