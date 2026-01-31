namespace Tindro.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tindro.Domain.Moderation;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReporterId).IsRequired();
        builder.Property(r => r.TargetUserId).IsRequired();
        builder.Property(r => r.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.ReportType).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Status).HasMaxLength(20).IsRequired().HasDefaultValue("Pending");
        builder.Property(r => r.ResolutionNotes).HasMaxLength(1000);

        // Relationships
        builder.HasOne(r => r.Reporter)
            .WithMany()
            .HasForeignKey(r => r.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.TargetUser)
            .WithMany()
            .HasForeignKey(r => r.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.TargetUserId);
        builder.HasIndex(r => r.ReportedAt);
    }
}