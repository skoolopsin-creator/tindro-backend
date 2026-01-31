namespace Tindro.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Chat;
using Tindro.Domain.Users;
public class MessageReadReceiptConfiguration : IEntityTypeConfiguration<MessageReadReceipt>
{
    public void Configure(EntityTypeBuilder<MessageReadReceipt> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.MessageId).IsRequired();
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.ReadAt).IsRequired();

        // Foreign keys
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => new { r.MessageId, r.UserId }).IsUnique();
        builder.HasIndex(r => r.ReadAt);
        builder.HasIndex(r => r.UserId);
    }
}

public class TypingIndicatorConfiguration : IEntityTypeConfiguration<TypingIndicator>
{
    public void Configure(EntityTypeBuilder<TypingIndicator> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ConversationId).IsRequired();
        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.StartedAt).IsRequired();
        builder.Property(t => t.IsActive).HasDefaultValue(true);

        // Foreign keys
        builder.HasOne<Conversation>()
            .WithMany()
            .HasForeignKey(t => t.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(t => new { t.ConversationId, t.UserId });
        builder.HasIndex(t => new { t.ConversationId, t.IsActive });
        builder.HasIndex(t => t.StartedAt);
    }
}

public class VoiceNoteConfiguration : IEntityTypeConfiguration<VoiceNote>
{
    public void Configure(EntityTypeBuilder<VoiceNote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.MessageId).IsRequired();
        builder.Property(v => v.UserId).IsRequired();
        builder.Property(v => v.AudioUrl).IsRequired();
        builder.Property(v => v.MimeType).IsRequired();
        builder.Property(v => v.DurationSeconds).HasDefaultValue(0);
        builder.Property(v => v.FileSizeBytes).HasDefaultValue(0);
        builder.Property(v => v.IsTranscribing).HasDefaultValue(false);
        builder.Property(v => v.PlayCount).HasDefaultValue(0);

        // Foreign keys
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(v => v.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(v => v.MessageId).IsUnique();
        builder.HasIndex(v => v.UserId);
        builder.HasIndex(v => v.CreatedAt);
    }
}

public class MessageExtensionConfiguration : IEntityTypeConfiguration<MessageExtension>
{
    public void Configure(EntityTypeBuilder<MessageExtension> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MessageId).IsRequired();
        builder.Property(m => m.DeliveryStatus).HasDefaultValue(MessageDeliveryStatus.Sending);
        builder.Property(m => m.ReadByCount).HasDefaultValue(0);
        builder.Property(m => m.IsEdited).HasDefaultValue(false);
        builder.Property(m => m.IsDeleted).HasDefaultValue(false);

        // Foreign keys
        builder.HasOne<Message>()
            .WithMany()
            .HasForeignKey(m => m.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        builder.HasMany<MessageReadReceipt>()
            .WithOne()
            .HasForeignKey("MessageExtensionId")
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => m.MessageId).IsUnique();
        builder.HasIndex(m => m.DeliveryStatus);
        builder.HasIndex(m => m.DeliveredAt);
    }
}

public class ConversationSettingsConfiguration : IEntityTypeConfiguration<ConversationSettings>
{
    public void Configure(EntityTypeBuilder<ConversationSettings> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ConversationId).IsRequired();
        builder.Property(s => s.UserId).IsRequired();
        builder.Property(s => s.MuteNotifications).HasDefaultValue(false);
        builder.Property(s => s.ShowReadReceipts).HasDefaultValue(true);
        builder.Property(s => s.ShowTypingIndicators).HasDefaultValue(true);
        builder.Property(s => s.AllowMediaMessages).HasDefaultValue(true);
        builder.Property(s => s.AllowVoiceNotes).HasDefaultValue(true);
        builder.Property(s => s.AllowCallInvites).HasDefaultValue(true);

        // Foreign keys
        builder.HasOne<Conversation>()
            .WithMany()
            .HasForeignKey(s => s.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(s => new { s.ConversationId, s.UserId }).IsUnique();
        builder.HasIndex(s => s.UserId);
    }
}
