namespace Tindro.Application.Verification.Dtos;

/// <summary>
/// Verification document upload DTO
/// </summary>
public class VerificationDocumentDto
{
    public Guid? Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? StorageUrl { get; set; }
    public string? MimeType { get; set; }
    public long FileSizeBytes { get; set; }
    public bool IsProcessed { get; set; }
    public string? ProcessingStatus { get; set; }
}

/// <summary>
/// Verification record request DTO
/// </summary>
public class SubmitVerificationRequestDto
{
    public string VerificationType { get; set; } = "id"; // "id", "phone", "email", "photo", "background"
    public List<VerificationDocumentDto> Documents { get; set; } = new();
    public string? AdditionalInfo { get; set; }
}

/// <summary>
/// Verification record response DTO
/// </summary>
public class VerificationStatusDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string VerificationType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? VerificationScore { get; set; }
    public List<VerificationDocumentDto> Documents { get; set; } = new();
}

/// <summary>
/// User verification result (multiple records)
/// </summary>
public class VerificationResultDto
{
    public Guid UserId { get; set; }
    public bool IsFullyVerified { get; set; }
    public bool IsIdVerified { get; set; }
    public bool IsPhotoVerified { get; set; }
    public bool IsBackgroundClear { get; set; }
    public int VerificationScore { get; set; } // 0-100
    public DateTime? LastVerificationDate { get; set; }
    public List<string> ActiveBadges { get; set; } = new();
    public List<VerificationStatusDto> VerificationRecords { get; set; } = new();
}

/// <summary>
/// User badge information
/// </summary>
public class BadgeDto
{
    public Guid Id { get; set; }
    public string BadgeType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BadgeIcon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public DateTime AwardedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Background check result DTO
/// </summary>
public class BackgroundCheckResultDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public string? Summary { get; set; }
    public bool HasCriminalRecord { get; set; }
    public bool HasSexOffenderRecord { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Request background check DTO
/// </summary>
public class RequestBackgroundCheckDto
{
    public string FullName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

/// <summary>
/// Verification attempt DTO (fraud detection)
/// </summary>
public class VerificationAttemptDto
{
    public Guid Id { get; set; }
    public int AttemptNumber { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? FlagReason { get; set; }
    public decimal? FraudScore { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Verification dashboard stats
/// </summary>
public class VerificationStatsDto
{
    public int TotalVerifiedUsers { get; set; }
    public int IdVerifiedCount { get; set; }
    public int PhotoVerifiedCount { get; set; }
    public int BackgroundClearCount { get; set; }
    public decimal VerificationRate { get; set; }
    public int PendingVerifications { get; set; }
    public int FlaggedAttempts { get; set; }
}

/// <summary>
/// Award badge request DTO
/// </summary>
public class AwardBadgeRequestDto
{
    public Guid UserId { get; set; }
    public string BadgeType { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Verification timeline event
/// </summary>
public class VerificationTimelineEventDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string? AdministratorNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Verification progress summary
/// </summary>
public class VerificationProgressDto
{
    public int TotalSteps { get; set; } = 3; // ID, Photo, Background
    public int CompletedSteps { get; set; }
    public List<VerificationStepDto> Steps { get; set; } = new();
    public decimal ProgressPercentage { get; set; }
    public string NextStep { get; set; } = string.Empty;
}

/// <summary>
/// Individual verification step
/// </summary>
public class VerificationStepDto
{
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "pending", "in-progress", "completed", "failed"
    public string Description { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
}
