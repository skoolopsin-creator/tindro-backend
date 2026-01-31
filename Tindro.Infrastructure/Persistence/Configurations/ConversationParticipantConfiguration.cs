using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Chat;

namespace Tindro.Infrastructure.Persistence.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConversationId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.JoinedAt).IsRequired();

        builder.HasIndex(x => new { x.ConversationId, x.UserId }).IsUnique();
    }
}
