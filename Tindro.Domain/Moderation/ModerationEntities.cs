namespace Tindro.Domain.Moderation;

using Tindro.Domain.Users;

/// <summary>
/// Represents a user-submitted report against another user.
/// </summary>
public class Report
{
    public Guid Id { get; set; }
    public Guid ReporterId { get; set; }
    public Guid TargetUserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ReportType { get; set; } = "profile"; // "profile", "message", "photo"
    public string Status { get; set; } = "Pending"; // "Pending", "Resolved", "Dismissed"
    public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }

    // Navigation properties
    public virtual User? Reporter { get; set; }
    public virtual User? TargetUser { get; set; }
}