using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Chat;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Text).IsRequired();
        builder.Property(x => x.CipherText);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SentAt).IsRequired();
        builder.Property(x => x.ConversationId).IsRequired();
        builder.Property(x => x.SenderId).IsRequired();
    }
}
