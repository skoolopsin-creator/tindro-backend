using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Phone).IsUnique();

        builder.HasOne(x => x.Profile)
               .WithOne(p => p.User)
               .HasForeignKey<Profile>(p => p.UserId);
    }
}
