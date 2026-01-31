namespace Tindro.Application.Verification.Interfaces;

using Tindro.Application.Verification.Dtos;
using Tindro.Domain.Verification;

/// <summary>
/// Verification repository interface
/// </summary>


/// <summary>
/// Verification service interface
/// </summary>
public interface IVerificationService
{
    // Verification workflow
    Task<VerificationStatusDto> SubmitVerificationAsync(Guid userId, SubmitVerificationRequestDto request);
    Task<VerificationResultDto> GetVerificationStatusAsync(Guid userId);
    Task<bool> IsUserVerifiedAsync(Guid userId);
    Task<int> GetVerificationScoreAsync(Guid userId); // 0-100

    // Document processing
    Task<VerificationDocumentDto> UploadVerificationDocumentAsync(Guid userId, Guid recordId, Stream fileStream, string documentType, string mimeType);
    Task ProcessVerificationDocumentAsync(Guid documentId);
    Task<bool> ValidateDocumentAsync(VerificationDocument document);

    // Approval workflow (admin)
    Task ApproveVerificationAsync(Guid recordId, Guid adminId);
    Task RejectVerificationAsync(Guid recordId, string reason);
    Task<List<VerificationStatusDto>> GetPendingVerificationsAsync(int limit = 50);

    // Fraud detection
    Task<VerificationAttemptDto> LogVerificationAttemptAsync(Guid userId, string ipAddress, string? deviceInfo);
    Task<decimal> CalculateFraudScoreAsync(Guid userId, VerificationAttempt attempt);
    Task<List<VerificationAttemptDto>> GetFlaggedAttemptsAsync();
    Task<bool> IsAttemptSuspiciousAsync(Guid userId, VerificationAttempt attempt);

    // Background checks
    Task<BackgroundCheckResultDto> RequestBackgroundCheckAsync(Guid userId, RequestBackgroundCheckDto request);
    Task<BackgroundCheckResultDto?> GetBackgroundCheckStatusAsync(Guid userId);
    Task<bool> IsBackgroundClearAsync(Guid userId);

    // Timeline and history
    Task<List<VerificationTimelineEventDto>> GetVerificationTimelineAsync(Guid userId);
    Task<VerificationProgressDto> GetVerificationProgressAsync(Guid userId);
}

/// <summary>
/// Badge service interface
/// </summary>
public interface IBadgeService
{
    // Badge management
    Task<BadgeDto> AwardBadgeAsync(Guid userId, string badgeType, string reason, DateTime? expiresAt = null);
    Task<bool> RemoveBadgeAsync(Guid userId, string badgeType);
    Task<List<BadgeDto>> GetUserBadgesAsync(Guid userId);
    Task<bool> UserHasBadgeAsync(Guid userId, string badgeType);

    // Automatic badge awarding
    Task AwardVerificationBadgeAsync(Guid userId);
    Task AwardBackgroundClearBadgeAsync(Guid userId);
    Task AwardCompletionBadgeAsync(Guid userId); // For highly complete profiles
    Task AwardCommunityBadgeAsync(Guid userId); // For community contributions

    // Badge expiry
    Task<List<BadgeDto>> GetExpiringBadgesAsync();
    Task ExpireBadgeAsync(Guid badgeId);
    Task RenewBadgeAsync(Guid badgeId);

    // Badge stats
    Task<VerificationStatsDto> GetBadgeStatsAsync();
    Task<int> GetUserBadgeCountAsync(Guid userId);
}

/// <summary>
/// Background check provider interface (for external service integration)
/// </summary>
public interface IBackgroundCheckProvider
{
    Task<BackgroundCheckResultDto> RequestCheckAsync(RequestBackgroundCheckDto request);
    Task<BackgroundCheckResultDto> GetCheckStatusAsync(string referenceId);
}
