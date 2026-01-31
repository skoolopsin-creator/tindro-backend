namespace Tindro.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Stories;

/// <summary>
/// Entity Framework configuration for Story entity
/// </summary>
public class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.ToTable("Stories");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.MediaUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Caption)
            .HasMaxLength(1000);

        builder.Property(s => s.MediaType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.DurationSeconds)
            .HasDefaultValue(5);

        builder.Property(s => s.BackgroundColor)
            .HasMaxLength(7);

        builder.Property(s => s.TextContent)
            .HasMaxLength(1000);

        builder.Property(s => s.TextColor)
            .HasMaxLength(7)
            .HasDefaultValue("#FFFFFF");

        builder.Property(s => s.AllowedUserIds)
            .HasColumnType("jsonb");

        builder.Property(s => s.VisibilityType)
            .HasDefaultValue("everyone")
            .HasMaxLength(20);

        builder.Property(s => s.ViewCount)
            .HasDefaultValue(0);

        builder.Property(s => s.LikeCount)
            .HasDefaultValue(0);

        builder.Property(s => s.CommentCount)
            .HasDefaultValue(0);

        builder.Property(s => s.ShareCount)
            .HasDefaultValue(0);

        builder.Property(s => s.AllowComments)
            .HasDefaultValue(true);

        builder.Property(s => s.AllowSharing)
            .HasDefaultValue(true);

        builder.Property(s => s.IsPublic)
            .HasDefaultValue(true);

        builder.Property(s => s.IsPinned)
            .HasDefaultValue(false);

        builder.Property(s => s.Position)
            .HasDefaultValue(0);

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.ExpiresAt);

        builder.Property(s => s.DeletedAt);

        // Indexes for common queries
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Stories_UserId");

        builder.HasIndex(s => new { s.UserId, s.CreatedAt })
            .HasDatabaseName("IX_Stories_UserId_CreatedAt");

        builder.HasIndex(s => new { s.IsDeleted, s.CreatedAt })
            .HasDatabaseName("IX_Stories_IsDeleted_CreatedAt");

        builder.HasIndex(s => s.ExpiresAt)
            .HasDatabaseName("IX_Stories_ExpiresAt");

        // Relationships
        builder.HasMany(s => s.Likes)
            .WithOne()
            .HasForeignKey(l => l.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Comments)
            .WithOne()
            .HasForeignKey(c => c.StoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Views)
            .WithOne()
            .HasForeignKey(v => v.StoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for StoryLike entity
/// </summary>
public class StoryLikeConfiguration : IEntityTypeConfiguration<StoryLike>
{
    public void Configure(EntityTypeBuilder<StoryLike> builder)
    {
        builder.ToTable("StoryLikes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.StoryId)
            .IsRequired();

        builder.Property(l => l.UserId)
            .IsRequired();

        builder.Property(l => l.ReactionType)
            .HasDefaultValue("like")
            .HasMaxLength(20);

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        // Prevent duplicate likes
        builder.HasIndex(l => new { l.StoryId, l.UserId })
            .IsUnique()
            .HasDatabaseName("IX_StoryLikes_StoryId_UserId_Unique");

        builder.HasIndex(l => l.CreatedAt)
            .HasDatabaseName("IX_StoryLikes_CreatedAt");
    }
}

/// <summary>
/// Entity Framework configuration for StoryComment entity
/// </summary>
public class StoryCommentConfiguration : IEntityTypeConfiguration<StoryComment>
{
    public void Configure(EntityTypeBuilder<StoryComment> builder)
    {
        builder.ToTable("StoryComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.StoryId)
            .IsRequired();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.LikeCount)
            .HasDefaultValue(0);

        builder.Property(c => c.IsEdited)
            .HasDefaultValue(false);

        builder.Property(c => c.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Indexes
        builder.HasIndex(c => c.StoryId)
            .HasDatabaseName("IX_StoryComments_StoryId");

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_StoryComments_UserId");

        builder.HasIndex(c => c.ParentCommentId)
            .HasDatabaseName("IX_StoryComments_ParentCommentId");

        builder.HasIndex(c => new { c.IsDeleted, c.CreatedAt })
            .HasDatabaseName("IX_StoryComments_IsDeleted_CreatedAt");

        // Self-referencing relationship for nested comments
        builder.HasMany<StoryComment>("Replies")
            .WithOne()
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// Entity Framework configuration for StoryCommentLike entity
/// </summary>
public class StoryCommentLikeConfiguration : IEntityTypeConfiguration<StoryCommentLike>
{
    public void Configure(EntityTypeBuilder<StoryCommentLike> builder)
    {
        builder.ToTable("StoryCommentLikes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.CommentId)
            .IsRequired();

        builder.Property(l => l.UserId)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        // Prevent duplicate likes on comments
        builder.HasIndex(l => new { l.CommentId, l.UserId })
            .IsUnique()
            .HasDatabaseName("IX_StoryCommentLikes_CommentId_UserId_Unique");
    }
}

/// <summary>
/// Entity Framework configuration for StoryView entity
/// </summary>
public class StoryViewConfiguration : IEntityTypeConfiguration<StoryView>
{
    public void Configure(EntityTypeBuilder<StoryView> builder)
    {
        builder.ToTable("StoryViews");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.StoryId)
            .IsRequired();

        builder.Property(v => v.UserId)
            .IsRequired();

        builder.Property(v => v.WatchPercentage)
            .HasDefaultValue(0);

        builder.Property(v => v.WatchTimeSeconds)
            .HasDefaultValue(0);

        builder.Property(v => v.IsCompleteView)
            .HasDefaultValue(false);

        builder.Property(v => v.ViewedAt)
            .IsRequired();

        // Indexes for efficient queries
        builder.HasIndex(v => v.StoryId)
            .HasDatabaseName("IX_StoryViews_StoryId");

        builder.HasIndex(v => v.UserId)
            .HasDatabaseName("IX_StoryViews_UserId");

        builder.HasIndex(v => new { v.StoryId, v.UserId })
            .HasDatabaseName("IX_StoryViews_StoryId_UserId");

        builder.HasIndex(v => v.ViewedAt)
            .HasDatabaseName("IX_StoryViews_ViewedAt");

        builder.HasIndex(v => v.IsCompleteView)
            .HasDatabaseName("IX_StoryViews_IsCompleteView");
    }
}

/// <summary>
/// Entity Framework configuration for StoryAnalytics entity
/// </summary>
public class StoryAnalyticsConfiguration : IEntityTypeConfiguration<StoryAnalytics>
{
    public void Configure(EntityTypeBuilder<StoryAnalytics> builder)
    {
        builder.ToTable("StoryAnalytics");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.StoryId)
            .IsRequired();

        builder.Property(a => a.TotalImpressions)
            .HasDefaultValue(0);

        builder.Property(a => a.UniqueViewers)
            .HasDefaultValue(0);

        builder.Property(a => a.AverageWatchTimeSeconds)
            .HasDefaultValue(0);

        builder.Property(a => a.AverageWatchPercentage)
            .HasDefaultValue(0);

        builder.Property(a => a.ShareCount)
            .HasDefaultValue(0);

        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        // Unique index on StoryId
        builder.HasIndex(a => a.StoryId)
            .IsUnique()
            .HasDatabaseName("IX_StoryAnalytics_StoryId_Unique");
    }
}
