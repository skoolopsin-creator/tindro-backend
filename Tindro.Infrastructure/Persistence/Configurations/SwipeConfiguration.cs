using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Match;

public class SwipeConfiguration : IEntityTypeConfiguration<Swipe>
{
    public void Configure(EntityTypeBuilder<Swipe> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.FromUserId, x.ToUserId }).IsUnique();
    }
}
