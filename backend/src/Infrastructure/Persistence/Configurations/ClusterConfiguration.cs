using Garimpo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garimpo.Infrastructure.Persistence.Configurations;

public sealed class ClusterConfiguration : IEntityTypeConfiguration<Cluster>
{
    public void Configure(EntityTypeBuilder<Cluster> builder)
    {
        builder.ToTable("clusters");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Label).IsRequired();
        builder.Property(c => c.CentroidAltitudeKm);
        builder.Property(c => c.CentroidInclinationDegrees);
        builder.Property(c => c.MemberCount);
        builder.Property(c => c.Density);
        builder.Property(c => c.CreatedAt);

        // Relacao 1:N - um aglomerado possui muitos detritos.
        // O backing field "_members" e usado pois a colecao e exposta como somente-leitura.
        builder.HasMany(c => c.Members)
            .WithOne(d => d.Cluster!)
            .HasForeignKey(d => d.ClusterId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Metadata
            .FindNavigation(nameof(Cluster.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
