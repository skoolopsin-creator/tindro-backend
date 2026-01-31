namespace Tindro.Domain.Verification;

using Tindro.Domain.Users;

/// <summary>
/// Verification records for user identity verification
/// </summary>
public class VerificationRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string VerificationType { get; set; } = "id"; // "id", "phone", "email", "photo", "background"
    public string Status { get; set; } = "pending"; // "pending", "approved", "rejected", "expired"
    public string? RejectionReason { get; set; }
    public int AttemptCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? VerificationScore { get; set; } // AI confidence score

    // Navigation
    public virtual User? User { get; set; }
    public virtual ICollection<VerificationDocument>? Documents { get; set; }
    public virtual ICollection<VerificationAttempt>? Attempts { get; set; }
}

/// <summary>
/// Uploaded verification documents
/// </summary>
public class VerificationDocument
{
    public Guid Id { get; set; }
    public Guid VerificationRecordId { get; set; }
    public string DocumentType { get; set; } = string.Empty; // "passport", "driver_license", "id_card", "selfie"
    public string StorageUrl { get; set; } = string.Empty;
    public string? MimeType { get; set; }
    public long FileSizeBytes { get; set; }
    public string? MetadataJson { get; set; } // Extracted document metadata
    public DateTime UploadedAt { get; set; }
    public bool IsProcessed { get; set; }
    public string? ProcessingStatus { get; set; } // "pending", "extracted", "verified", "rejected"

    // Navigation
    public virtual VerificationRecord? VerificationRecord { get; set; }
}

/// <summary>
/// User verification badges (visual indicators of verification)
/// </summary>
public class UserVerificationBadge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BadgeType { get; set; } = string.Empty; // "verified", "id_verified", "selfie_verified", "background_clear", "top_contributor"
    public string DisplayName { get; set; } = string.Empty; // "Verified", "ID Verified", etc.
    public string BadgeIcon { get; set; } = string.Empty; // URL or icon identifier
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; } // Display priority (higher = show first)
    public bool IsActive { get; set; } = true;
    public DateTime AwardedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Criteria { get; set; } // JSON description of how badge was earned
    public int DisplayOrder { get; set; } // Order in which badges appear on profile

    // Navigation
    public virtual User? User { get; set; }
}

/// <summary>
/// Verification attempt records for fraud detection
/// </summary>
public class VerificationAttempt
{
    public Guid Id { get; set; }
    public Guid VerificationRecordId { get; set; }
    public Guid UserId { get; set; }
    public int AttemptNumber { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string Status { get; set; } = "pending"; // "pending", "approved", "rejected", "flagged"
    public string? FlagReason { get; set; } // Fraud detection reason
    public decimal? FraudScore { get; set; } // 0-1, higher = more suspicious
    public DateTime CreatedAt { get; set; }
    public string? AdditionalData { get; set; } // JSON extra verification data

    // Navigation
    public virtual VerificationRecord? VerificationRecord { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Background check results
/// </summary>
public class BackgroundCheckResult
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Status { get; set; } = "pending"; // "pending", "clear", "issues_found", "failed"
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderReferenceId { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Summary { get; set; }
    public bool HasCriminalRecord { get; set; }
    public bool HasSexOffenderRecord { get; set; }
    public string? Findings { get; set; } // JSON detailed findings
    public DateTime? ExpiresAt { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}

/// <summary>
/// Verification activity log
/// </summary>
public class VerificationLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty; // "submitted", "approved", "rejected", "expired", "renewed"
    public string Details { get; set; } = string.Empty;
    public string? AdministratorNotes { get; set; }
    public Guid? AdministratorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? IpAddress { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}
