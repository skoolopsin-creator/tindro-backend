using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Location;

namespace Tindro.Infrastructure.Persistence.Configurations;

public class UserLocationConfiguration : IEntityTypeConfiguration<UserLocation>
{
    public void Configure(EntityTypeBuilder<UserLocation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Geohash).IsRequired().HasMaxLength(12);
        builder.Property(x => x.CityId).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();

        // Indexes for queries
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.Geohash);
        builder.HasIndex(x => x.ExpiresAt);

        // TTL handling - delete at ExpiresAt
        builder.ToTable("user_locations", t => t.HasCheckConstraint("ck_expires_at", "\"ExpiresAt\" > \"UpdatedAt\""));
    }
}

public class CrossedPathConfiguration : IEntityTypeConfiguration<CrossedPath>
{
    public void Configure(EntityTypeBuilder<CrossedPath> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.User1Id).IsRequired();
        builder.Property(x => x.User2Id).IsRequired();
        builder.Property(x => x.Geohash).IsRequired().HasMaxLength(12);
        builder.Property(x => x.CrossedAt).IsRequired();
        builder.Property(x => x.ExpiresAt).IsRequired();

        // Prevent duplicates - only record once per pair
        builder.HasIndex(x => new { x.User1Id, x.User2Id }).IsUnique();

        // Indexes for queries
        builder.HasIndex(x => x.ExpiresAt);

        // Ensure User1Id < User2Id for consistency
        builder.ToTable("crossed_paths", t => t.HasCheckConstraint("ck_user_order", "\"User1Id\" < \"User2Id\""));
    }
}

public class LocationPrivacyPreferencesConfiguration : IEntityTypeConfiguration<LocationPrivacyPreferences>
{
    public void Configure(EntityTypeBuilder<LocationPrivacyPreferences> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.IsLocationEnabled).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.HideDistance).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.IsPaused).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.VerifiedOnlyMap).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => x.UserId).IsUnique();
    }
}

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Country).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Location).IsRequired();

        // PostGIS Point geometry
        // Requires PostGIS extension: CREATE EXTENSION IF NOT EXISTS postgis;
        builder.HasIndex(x => new { x.Name, x.Country }).IsUnique();
    }
}
