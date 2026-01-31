using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Recommendations;

namespace Tindro.Infrastructure.Persistence.Configurations;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId).IsRequired();
        builder.HasIndex(p => p.UserId).IsUnique();

        builder.Property(p => p.MinAgePreference).IsRequired();
        builder.Property(p => p.MaxAgePreference).IsRequired();
        builder.Property(p => p.MaxDistancePreference).IsRequired();

        builder.Property(p => p.InterestCategories)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToList());

        builder.Property(p => p.PersonalityTraits)
            .HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", System.StringSplitOptions.RemoveEmptyEntries).ToList());

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.ToTable("user_preferences");
    }
}

public class RecommendationScoreConfiguration : IEntityTypeConfiguration<RecommendationScore>
{
    public void Configure(EntityTypeBuilder<RecommendationScore> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.RecommendedUserId).IsRequired();

        builder.HasIndex(r => new { r.UserId, r.RecommendedUserId }).IsUnique();
        builder.HasIndex(r => new { r.UserId, r.OverallScore }).IsDescending(false, true);
        builder.HasIndex(r => r.ExpiresAt);

        builder.Property(r => r.AgeCompatibility).HasPrecision(5, 2);
        builder.Property(r => r.LocationCompatibility).HasPrecision(5, 2);
        builder.Property(r => r.InterestCompatibility).HasPrecision(5, 2);
        builder.Property(r => r.LifestyleCompatibility).HasPrecision(5, 2);
        builder.Property(r => r.ProfileCompleteness).HasPrecision(5, 2);
        builder.Property(r => r.VerificationScore).HasPrecision(5, 2);
        builder.Property(r => r.OverallScore).HasPrecision(5, 2);

        builder.Property(r => r.CalculatedAt).IsRequired();
        builder.Property(r => r.ExpiresAt).IsRequired();

        builder.ToTable("recommendation_scores");
    }
}

public class UserInterestConfiguration : IEntityTypeConfiguration<UserInterest>
{
    public void Configure(EntityTypeBuilder<UserInterest> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.UserId).IsRequired();
        builder.Property(i => i.InterestName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Category).IsRequired().HasMaxLength(50);

        builder.HasIndex(i => new { i.UserId, i.InterestName }).IsUnique();
        builder.HasIndex(i => i.Category);

        builder.Property(i => i.ConfidenceScore).IsRequired();
        builder.Property(i => i.AddedAt).IsRequired();

        builder.ToTable("user_interests");
    }
}

public class SkipProfileConfiguration : IEntityTypeConfiguration<SkipProfile>
{
    public void Configure(EntityTypeBuilder<SkipProfile> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.SkippedUserId).IsRequired();

        builder.HasIndex(s => new { s.UserId, s.SkippedUserId }).IsUnique();
        builder.HasIndex(s => s.ExpiresAt);

        builder.Property(s => s.Reason).HasMaxLength(50);
        builder.Property(s => s.SkippedAt).IsRequired();
        builder.Property(s => s.ExpiresAt).IsRequired();

        builder.ToTable("skip_profiles");
    }
}

public class RecommendationFeedbackConfiguration : IEntityTypeConfiguration<RecommendationFeedback>
{
    public void Configure(EntityTypeBuilder<RecommendationFeedback> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.UserId).IsRequired();
        builder.Property(f => f.RecommendedUserId).IsRequired();
        builder.Property(f => f.FeedbackType).IsRequired().HasMaxLength(50);

        builder.HasIndex(f => new { f.UserId, f.CreatedAt }).IsDescending(false, true);
        builder.HasIndex(f => f.FeedbackType);

        builder.Property(f => f.CreatedAt).IsRequired();

        builder.ToTable("recommendation_feedback");
    }
}
