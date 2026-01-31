using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Domain.Verification;

namespace Tindro.Application.Verification.Interfaces
{
    public interface IVerificationRepository
    {
        // VerificationRecord operations
        Task<VerificationRecord?> GetVerificationRecordAsync(Guid recordId);
        Task<List<VerificationRecord>> GetUserVerificationRecordsAsync(Guid userId);
        Task<VerificationRecord?> GetLatestVerificationAsync(Guid userId, string verificationType);
        Task<VerificationRecord> CreateVerificationRecordAsync(VerificationRecord record);
        Task<VerificationRecord> UpdateVerificationRecordAsync(VerificationRecord record);
        Task ApproveVerificationAsync(Guid recordId, Guid reviewerId);
        Task RejectVerificationAsync(Guid recordId, string reason);

        // VerificationDocument operations
        Task<VerificationDocument> AddDocumentAsync(VerificationDocument document);
        Task<List<VerificationDocument>> GetVerificationDocumentsAsync(Guid recordId);
        Task DeleteDocumentAsync(Guid documentId);

        // UserVerificationBadge operations
        Task<List<UserVerificationBadge>> GetUserBadgesAsync(Guid userId);
        Task<UserVerificationBadge?> GetBadgeAsync(Guid badgeId);
        Task<UserVerificationBadge> AwardBadgeAsync(UserVerificationBadge badge);
        Task<bool> RemoveBadgeAsync(Guid badgeId);
        Task<bool> UserHasBadgeAsync(Guid userId, string badgeType);

        // VerificationAttempt operations
        Task<VerificationAttempt> LogAttemptAsync(VerificationAttempt attempt);
        Task<List<VerificationAttempt>> GetUserAttemptsAsync(Guid userId);
        Task<int> GetFailedAttemptsCountAsync(Guid userId, TimeSpan timeWindow);

        // BackgroundCheckResult operations
        Task<BackgroundCheckResult?> GetLatestBackgroundCheckAsync(Guid userId);
        Task<BackgroundCheckResult> CreateBackgroundCheckAsync(BackgroundCheckResult result);
        Task<BackgroundCheckResult> UpdateBackgroundCheckAsync(BackgroundCheckResult result);

        // VerificationLog operations
        Task<VerificationLog> LogActionAsync(VerificationLog log);
        Task<List<VerificationLog>> GetUserVerificationLogAsync(Guid userId);
        Task<List<VerificationRecord>> GetAllPendingAsync(int limit);
        Task<List<UserVerificationBadge>> GetBadgesExpiringBeforeAsync(DateTime threshold);
        Task<List<UserVerificationBadge>> GetAllBadgesAsync();
        Task<UserVerificationBadge> UpdateBadgeAsync(UserVerificationBadge badge);
        Task<List<VerificationRecord>> GetAllVerificationRecordsAsync();
        Task<List<VerificationAttempt>> GetAllVerificationAttemptsAsync();
        Task<VerificationDocument?> GetVerificationDocumentAsync(Guid documentId);
        Task<List<VerificationAttempt>> GetFlaggedAttemptsAsync(decimal threshold);
        Task<VerificationDocument> UpdateVerificationDocumentAsync(VerificationDocument document);

    }
}
