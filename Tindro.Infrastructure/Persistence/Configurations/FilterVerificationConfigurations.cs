namespace Tindro.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Discovery;
using Tindro.Domain.Verification;

/// <summary>
/// EF Core configurations for discovery filter entities
/// </summary>
public class FilterPreferencesConfiguration : IEntityTypeConfiguration<FilterPreferences>
{
    public void Configure(EntityTypeBuilder<FilterPreferences> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.UserId).IsRequired();
        builder.Property(f => f.Name).HasMaxLength(100).IsRequired();
        builder.Property(f => f.MinAge).HasDefaultValue(18);
        builder.Property(f => f.MaxAge).HasDefaultValue(100);
        builder.Property(f => f.CreatedAt).ValueGeneratedOnAdd();
        
        // Indexes
        builder.HasIndex(f => f.UserId).HasDatabaseName("IX_FilterPreferences_UserId");
        builder.HasIndex(f => new { f.UserId, f.IsActive }).HasDatabaseName("IX_FilterPreferences_UserId_IsActive");
    }
}

public class FilterCriteriaConfiguration : IEntityTypeConfiguration<FilterCriteria>
{
    public void Configure(EntityTypeBuilder<FilterCriteria> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.FilterPreferencesId).IsRequired();
        builder.Property(c => c.CriteriaType).HasMaxLength(50).IsRequired();
        builder.Property(c => c.Operator).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Value).HasMaxLength(500);
        builder.Property(c => c.Priority).HasDefaultValue(5);
        
        // Relationships
        builder.HasOne(c => c.FilterPreferences)
            .WithMany()
            .HasForeignKey(c => c.FilterPreferencesId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(c => c.FilterPreferencesId).HasDatabaseName("IX_FilterCriteria_FilterPreferencesId");
    }
}

public class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
{
    public void Configure(EntityTypeBuilder<SavedFilter> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(500);
        builder.Property(s => s.FilterData).HasColumnType("jsonb");
        builder.Property(s => s.CreatedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(s => s.FilterPreferences)
            .WithMany()
            .HasForeignKey(s => s.FilterPreferencesId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => s.UserId).HasDatabaseName("IX_SavedFilter_UserId");
        builder.HasIndex(s => new { s.UserId, s.IsDefault }).HasDatabaseName("IX_SavedFilter_UserId_IsDefault");
    }
}

public class FilterApplicationHistoryConfiguration : IEntityTypeConfiguration<FilterApplicationHistory>
{
    public void Configure(EntityTypeBuilder<FilterApplicationHistory> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.UserId).IsRequired();
        builder.Property(h => h.FilterPreferencesId).IsRequired();
        builder.Property(h => h.AppliedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(h => h.FilterPreferences)
            .WithMany()
            .HasForeignKey(h => h.FilterPreferencesId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(h => h.User)
            .WithMany()
            .HasForeignKey(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes - important for analytics queries
        builder.HasIndex(h => h.UserId).HasDatabaseName("IX_FilterApplicationHistory_UserId");
        builder.HasIndex(h => new { h.UserId, h.AppliedAt }).HasDatabaseName("IX_FilterApplicationHistory_UserId_AppliedAt");
        builder.HasIndex(h => h.ExpiresAt).HasDatabaseName("IX_FilterApplicationHistory_ExpiresAt");
    }
}

/// <summary>
/// EF Core configurations for verification entities
/// </summary>
public class VerificationRecordConfiguration : IEntityTypeConfiguration<VerificationRecord>
{
    public void Configure(EntityTypeBuilder<VerificationRecord> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.VerificationType).HasMaxLength(50).IsRequired();
        builder.Property(v => v.Status).HasMaxLength(20).IsRequired();
        builder.Property(v => v.RejectionReason).HasMaxLength(500);
        builder.Property(v => v.SubmittedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Documents)
            .WithOne(d => d.VerificationRecord)
            .HasForeignKey(d => d.VerificationRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.Attempts)
            .WithOne(a => a.VerificationRecord)
            .HasForeignKey(a => a.VerificationRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.UserId).HasDatabaseName("IX_VerificationRecord_UserId");
        builder.HasIndex(v => new { v.UserId, v.Status }).HasDatabaseName("IX_VerificationRecord_UserId_Status");
        builder.HasIndex(v => v.Status).HasDatabaseName("IX_VerificationRecord_Status");
        builder.HasIndex(v => v.ExpiresAt).HasDatabaseName("IX_VerificationRecord_ExpiresAt");
    }
}

public class VerificationDocumentConfiguration : IEntityTypeConfiguration<VerificationDocument>
{
    public void Configure(EntityTypeBuilder<VerificationDocument> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.VerificationRecordId).IsRequired();
        builder.Property(d => d.DocumentType).HasMaxLength(50).IsRequired();
        builder.Property(d => d.StorageUrl).HasMaxLength(500).IsRequired();
        builder.Property(d => d.MimeType).HasMaxLength(50);
        builder.Property(d => d.MetadataJson).HasColumnType("jsonb");
        builder.Property(d => d.UploadedAt).ValueGeneratedOnAdd();

        // Indexes
        builder.HasIndex(d => d.VerificationRecordId).HasDatabaseName("IX_VerificationDocument_VerificationRecordId");
        builder.HasIndex(d => d.ProcessingStatus).HasDatabaseName("IX_VerificationDocument_ProcessingStatus");
    }
}

public class UserVerificationBadgeConfiguration : IEntityTypeConfiguration<UserVerificationBadge>
{
    public void Configure(EntityTypeBuilder<UserVerificationBadge> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserId).IsRequired();
        builder.Property(b => b.BadgeType).HasMaxLength(50).IsRequired();
        builder.Property(b => b.DisplayName).HasMaxLength(100).IsRequired();
        builder.Property(b => b.BadgeIcon).HasMaxLength(255);
        builder.Property(b => b.Description).HasMaxLength(500);
        builder.Property(b => b.Criteria).HasColumnType("jsonb");
        builder.Property(b => b.AwardedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(b => b.UserId).HasDatabaseName("IX_UserVerificationBadge_UserId");
        builder.HasIndex(b => new { b.UserId, b.IsActive }).HasDatabaseName("IX_UserVerificationBadge_UserId_IsActive");
        builder.HasIndex(b => b.BadgeType).HasDatabaseName("IX_UserVerificationBadge_BadgeType");
        builder.HasIndex(b => b.ExpiresAt).HasDatabaseName("IX_UserVerificationBadge_ExpiresAt");
    }
}

public class VerificationAttemptConfiguration : IEntityTypeConfiguration<VerificationAttempt>
{
    public void Configure(EntityTypeBuilder<VerificationAttempt> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.VerificationRecordId).IsRequired();
        builder.Property(a => a.UserId).IsRequired();
        builder.Property(a => a.IpAddress).HasMaxLength(45).IsRequired();
        builder.Property(a => a.DeviceInfo).HasMaxLength(500);
        builder.Property(a => a.Status).HasMaxLength(20).IsRequired();
        builder.Property(a => a.FlagReason).HasMaxLength(255);
        builder.Property(a => a.FraudScore).HasPrecision(3, 2);
        builder.Property(a => a.AdditionalData).HasColumnType("jsonb");
        builder.Property(a => a.CreatedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(a => a.VerificationRecord)
            .WithMany(v => v.Attempts)
            .HasForeignKey(a => a.VerificationRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.UserId).HasDatabaseName("IX_VerificationAttempt_UserId");
        builder.HasIndex(a => a.Status).HasDatabaseName("IX_VerificationAttempt_Status");
        builder.HasIndex(a => new { a.UserId, a.CreatedAt }).HasDatabaseName("IX_VerificationAttempt_UserId_CreatedAt");
    }
}

public class BackgroundCheckResultConfiguration : IEntityTypeConfiguration<BackgroundCheckResult>
{
    public void Configure(EntityTypeBuilder<BackgroundCheckResult> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserId).IsRequired();
        builder.Property(b => b.Status).HasMaxLength(20).IsRequired();
        builder.Property(b => b.ProviderName).HasMaxLength(100).IsRequired();
        builder.Property(b => b.ProviderReferenceId).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Summary).HasMaxLength(1000);
        builder.Property(b => b.Findings).HasColumnType("jsonb");
        builder.Property(b => b.RequestedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(b => b.UserId).HasDatabaseName("IX_BackgroundCheckResult_UserId");
        builder.HasIndex(b => new { b.UserId, b.Status }).HasDatabaseName("IX_BackgroundCheckResult_UserId_Status");
        builder.HasIndex(b => b.ExpiresAt).HasDatabaseName("IX_BackgroundCheckResult_ExpiresAt");
    }
}

public class VerificationLogConfiguration : IEntityTypeConfiguration<VerificationLog>
{
    public void Configure(EntityTypeBuilder<VerificationLog> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.UserId).IsRequired();
        builder.Property(l => l.Action).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Details).HasMaxLength(500);
        builder.Property(l => l.AdministratorNotes).HasMaxLength(1000);
        builder.Property(l => l.CreatedAt).ValueGeneratedOnAdd();

        // Relationships
        builder.HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(l => l.UserId).HasDatabaseName("IX_VerificationLog_UserId");
        builder.HasIndex(l => new { l.UserId, l.CreatedAt }).HasDatabaseName("IX_VerificationLog_UserId_CreatedAt");
    }
}
