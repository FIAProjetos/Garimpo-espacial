using Garimpo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garimpo.Infrastructure.Persistence.Configurations;

public sealed class DebrisConfiguration : IEntityTypeConfiguration<Debris>
{
    public void Configure(EntityTypeBuilder<Debris> builder)
    {
        builder.ToTable("debris");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NoradId).IsRequired();
        builder.HasIndex(d => d.NoradId).IsUnique();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(d => d.Line1)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(d => d.Line2)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(d => d.InclinationDegrees);
        builder.Property(d => d.Eccentricity);
        builder.Property(d => d.MeanMotionRevsPerDay);
        builder.Property(d => d.AltitudeKm);

        builder.Property(d => d.Classification)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(d => d.CapturedAt);

        builder.HasIndex(d => d.AltitudeKm);
        builder.HasIndex(d => d.ClusterId);
    }
}
