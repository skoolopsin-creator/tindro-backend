using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Verification.Interfaces;
using Tindro.Domain.Verification;

namespace Tindro.Infrastructure.Persistence.Repositories
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly QueryDbContext _context;

        public VerificationRepository(QueryDbContext context)
        {
            _context = context;
        }

        // VerificationRecord operations
        public async Task<VerificationRecord?> GetVerificationRecordAsync(Guid recordId)
        {
            return await _context.VerificationRecords
                .Include(r => r.Documents)
                .Include(r => r.Attempts)
                .FirstOrDefaultAsync(r => r.Id == recordId);
        }

        public async Task<List<VerificationRecord>> GetUserVerificationRecordsAsync(Guid userId)
        {
            return await _context.VerificationRecords
                .Include(r => r.Documents)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
        }

        public async Task<VerificationRecord?> GetLatestVerificationAsync(Guid userId, string verificationType)
        {
            return await _context.VerificationRecords
                .Where(r => r.UserId == userId && r.VerificationType == verificationType)
                .OrderByDescending(r => r.SubmittedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<VerificationRecord> CreateVerificationRecordAsync(VerificationRecord record)
        {
            _context.VerificationRecords.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task<VerificationRecord> UpdateVerificationRecordAsync(VerificationRecord record)
        {
            _context.VerificationRecords.Update(record);
            await _context.SaveChangesAsync();
            return record;
        }

        public async Task ApproveVerificationAsync(Guid recordId, Guid reviewerId)
        {
            var record = await _context.VerificationRecords.FindAsync(recordId);
            if (record != null)
            {
                record.Status = "approved";
                record.ReviewedAt = DateTime.UtcNow;
                record.ReviewedBy = reviewerId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RejectVerificationAsync(Guid recordId, string reason)
        {
            var record = await _context.VerificationRecords.FindAsync(recordId);
            if (record != null)
            {
                record.Status = "rejected";
                record.RejectionReason = reason;
                record.ReviewedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // VerificationDocument operations
        public async Task<VerificationDocument> AddDocumentAsync(VerificationDocument document)
        {
            _context.VerificationDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<List<VerificationDocument>> GetVerificationDocumentsAsync(Guid recordId)
        {
            return await _context.VerificationDocuments
                .Where(d => d.VerificationRecordId == recordId)
                .ToListAsync();
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            var document = await _context.VerificationDocuments.FindAsync(documentId);
            if (document != null)
            {
                _context.VerificationDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        // UserVerificationBadge operations
        public async Task<List<UserVerificationBadge>> GetUserBadgesAsync(Guid userId)
        {
            return await _context.UserVerificationBadges
                .Where(b => b.UserId == userId && b.IsActive)
                .OrderByDescending(b => b.Priority)
                .ToListAsync();
        }

        public async Task<UserVerificationBadge?> GetBadgeAsync(Guid badgeId)
        {
            return await _context.UserVerificationBadges.FindAsync(badgeId);
        }

        public async Task<UserVerificationBadge> AwardBadgeAsync(UserVerificationBadge badge)
        {
            _context.UserVerificationBadges.Add(badge);
            await _context.SaveChangesAsync();
            return badge;
        }

        public async Task<bool> RemoveBadgeAsync(Guid badgeId)
        {
            var badge = await _context.UserVerificationBadges.FindAsync(badgeId);
            if (badge != null)
            {
                badge.IsActive = false;
                _context.UserVerificationBadges.Update(badge);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UserHasBadgeAsync(Guid userId, string badgeType)
        {
            return await _context.UserVerificationBadges
                .AnyAsync(b => b.UserId == userId && b.BadgeType == badgeType && b.IsActive);
        }

        // VerificationAttempt operations
        public async Task<VerificationAttempt> LogAttemptAsync(VerificationAttempt attempt)
        {
            _context.VerificationAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<List<VerificationAttempt>> GetUserAttemptsAsync(Guid userId)
        {
            return await _context.VerificationAttempts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
public async Task<List<VerificationAttempt>> GetFlaggedAttemptsAsync(decimal threshold)
{
    return await _context.VerificationAttempts
        .Where(a => a.FraudScore >= threshold)
        .OrderByDescending(a => a.FraudScore)
        .ToListAsync();
}
public async Task<VerificationDocument> UpdateVerificationDocumentAsync(VerificationDocument document)
{
    _context.VerificationDocuments.Update(document);
    await _context.SaveChangesAsync();
    return document;
}

        public async Task<int> GetFailedAttemptsCountAsync(Guid userId, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);
            return await _context.VerificationAttempts
                .CountAsync(a => a.UserId == userId && a.Status == "rejected" && a.CreatedAt > cutoffTime);
        }

        // BackgroundCheckResult operations
        public async Task<BackgroundCheckResult?> GetLatestBackgroundCheckAsync(Guid userId)
        {
            return await _context.BackgroundCheckResults
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.RequestedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<BackgroundCheckResult> CreateBackgroundCheckAsync(BackgroundCheckResult result)
        {
            _context.BackgroundCheckResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<BackgroundCheckResult> UpdateBackgroundCheckAsync(BackgroundCheckResult result)
        {
            _context.BackgroundCheckResults.Update(result);
            await _context.SaveChangesAsync();
            return result;
        }

        // VerificationLog operations
        public async Task<VerificationLog> LogActionAsync(VerificationLog log)
        {
            _context.VerificationLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<List<VerificationLog>> GetUserVerificationLogAsync(Guid userId)
        {
            return await _context.VerificationLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<VerificationRecord>> GetAllPendingAsync(int limit)
        {
            return await _context.VerificationRecords
                .Where(r => r.Status == "pending")
                .OrderBy(r => r.SubmittedAt)
                .Take(limit)
                .ToListAsync();
        }
        public async Task<List<UserVerificationBadge>> GetBadgesExpiringBeforeAsync(DateTime date)
        {
            return await _context.UserVerificationBadges
                .Where(b => b.IsActive && b.ExpiresAt != null && b.ExpiresAt <= date)
                .ToListAsync();
        }

        public async Task<List<UserVerificationBadge>> GetAllBadgesAsync()
        {
            return await _context.UserVerificationBadges.ToListAsync();
        }

        // VerificationRepository.cs

public async Task<UserVerificationBadge> UpdateBadgeAsync(UserVerificationBadge badge)
{
    _context.UserVerificationBadges.Update(badge);
    await _context.SaveChangesAsync();
    return badge;
}

public async Task<List<VerificationRecord>> GetAllVerificationRecordsAsync()
{
    return await _context.VerificationRecords.ToListAsync();
}

public async Task<List<VerificationAttempt>> GetAllVerificationAttemptsAsync()
{
    return await _context.VerificationAttempts.ToListAsync();
}
public async Task<VerificationDocument?> GetVerificationDocumentAsync(Guid documentId)
{
    return await _context.VerificationDocuments
        .Include(d => d.VerificationRecord)
        .FirstOrDefaultAsync(d => d.Id == documentId);
}

    }

}
