namespace Tindro.Application.Verification.Services;

using Tindro.Application.Verification.Interfaces;
using Tindro.Application.Verification.Dtos;
using Tindro.Domain.Verification;

public class VerificationService : IVerificationService
{
    private readonly IVerificationRepository _verificationRepository;
    private readonly IBadgeService _badgeService;

    public VerificationService(IVerificationRepository verificationRepository, IBadgeService badgeService)
    {
        _verificationRepository = verificationRepository;
        _badgeService = badgeService;
    }

    public async Task<VerificationStatusDto> SubmitVerificationAsync(Guid userId, SubmitVerificationRequestDto request)
    {
        var record = new VerificationRecord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            VerificationType = request.VerificationType,
            Status = "pending",
            SubmittedAt = DateTime.UtcNow
        };

        var created = await _verificationRepository.CreateVerificationRecordAsync(record);

        // Add documents
        foreach (var doc in request.Documents)
        {
            var verificationDoc = new VerificationDocument
            {
                Id = Guid.NewGuid(),
                VerificationRecordId = created.Id,
                DocumentType = doc.DocumentType,
                StorageUrl = doc.StorageUrl ?? string.Empty,
                MimeType = doc.MimeType,
                FileSizeBytes = doc.FileSizeBytes,
                UploadedAt = DateTime.UtcNow
            };

            await _verificationRepository.AddDocumentAsync(verificationDoc);
        }

        // Log action
        await _verificationRepository.LogActionAsync(new VerificationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = "submitted",
            Details = $"User submitted {request.VerificationType} verification",
            CreatedAt = DateTime.UtcNow
        });

        return await GetVerificationStatusByRecordAsync(created.Id);
    }

    public async Task<VerificationResultDto> GetVerificationStatusAsync(Guid userId)
    {
        var records = await _verificationRepository.GetUserVerificationRecordsAsync(userId);
        var badges = await _verificationRepository.GetUserBadgesAsync(userId);

        var idVerified = records.Any(r => r.VerificationType == "id" && r.Status == "approved");
        var photoVerified = records.Any(r => r.VerificationType == "photo" && r.Status == "approved");
        var backgroundCheck = await _verificationRepository.GetLatestBackgroundCheckAsync(userId);
        var backgroundClear = backgroundCheck?.Status == "clear";

        var verificationScore = (idVerified ? 40 : 0) + (photoVerified ? 30 : 0) + (backgroundClear ? 30 : 0);

        return new VerificationResultDto
        {
            UserId = userId,
            IsFullyVerified = idVerified && photoVerified && backgroundClear,
            IsIdVerified = idVerified,
            IsPhotoVerified = photoVerified,
            IsBackgroundClear = backgroundClear,
            VerificationScore = verificationScore,
            LastVerificationDate = records.Max(r => r.SubmittedAt),
            ActiveBadges = badges.Where(b => b.IsActive).Select(b => b.BadgeType).ToList(),
            VerificationRecords = records.Select(MapRecordToDto).ToList()
        };
    }

    public async Task<bool> IsUserVerifiedAsync(Guid userId)
    {
        var status = await GetVerificationStatusAsync(userId);
        return status.IsFullyVerified;
    }

    public async Task<int> GetVerificationScoreAsync(Guid userId)
    {
        var status = await GetVerificationStatusAsync(userId);
        return status.VerificationScore;
    }

    public async Task<VerificationDocumentDto> UploadVerificationDocumentAsync(Guid userId, Guid recordId, Stream fileStream, string documentType, string mimeType)
    {
        var record = await _verificationRepository.GetVerificationRecordAsync(recordId);
        if (record == null || record.UserId != userId)
            throw new UnauthorizedAccessException("Cannot access this verification record");

        // In real implementation, would upload to storage service
        var document = new VerificationDocument
        {
            Id = Guid.NewGuid(),
            VerificationRecordId = recordId,
            DocumentType = documentType,
            StorageUrl = $"/storage/verifications/{recordId}/{Guid.NewGuid()}",
            MimeType = mimeType,
            FileSizeBytes = fileStream.Length,
            UploadedAt = DateTime.UtcNow
        };

        var created = await _verificationRepository.AddDocumentAsync(document);
        return new VerificationDocumentDto
        {
            Id = created.Id,
            DocumentType = created.DocumentType,
            StorageUrl = created.StorageUrl,
            MimeType = created.MimeType,
            FileSizeBytes = created.FileSizeBytes,
            IsProcessed = created.IsProcessed
        };
    }

 public async Task ProcessVerificationDocumentAsync(Guid documentId)
{
    var doc = await _verificationRepository.GetVerificationDocumentAsync(documentId);
    if (doc == null)
        throw new KeyNotFoundException("Document not found");

    var isValid = await ValidateDocumentAsync(doc);

    doc.IsProcessed = true;
    doc.ProcessingStatus = isValid ? "approved" : "rejected";

    await _verificationRepository.UpdateVerificationDocumentAsync(doc);

    // Auto-approve record if all documents approved
    var record = doc.VerificationRecord!;
    if (record.Documents.All(d => d.IsProcessed && d.ProcessingStatus == "approved"))
    {
        record.Status = "approved";
        record.ReviewedAt = DateTime.UtcNow;
        await _verificationRepository.UpdateVerificationRecordAsync(record);
    }
}


    public async Task<bool> ValidateDocumentAsync(VerificationDocument document)
    {
        // Validate document format, size, content, etc.
        if (document.FileSizeBytes > 10_000_000) // 10MB limit
            return false;
        
        if (!new[] { "image/jpeg", "image/png", "application/pdf" }.Contains(document.MimeType))
            return false;

        return true;
    }

    public async Task ApproveVerificationAsync(Guid recordId, Guid adminId)
    {
        await _verificationRepository.ApproveVerificationAsync(recordId, adminId);
        
        var record = await _verificationRepository.GetVerificationRecordAsync(recordId);
        if (record != null)
        {
            // Award badge if ID verified
            if (record.VerificationType == "id")
            {
                await _badgeService.AwardBadgeAsync(record.UserId, "id_verified", "User ID verified");
            }
        }
    }

    public async Task RejectVerificationAsync(Guid recordId, string reason)
    {
        await _verificationRepository.RejectVerificationAsync(recordId, reason);
    }

   public async Task<List<VerificationStatusDto>> GetPendingVerificationsAsync(int limit = 50)
    {
        var records = await _verificationRepository.GetAllPendingAsync(limit);

        return records.Select(MapRecordToDto).ToList();
    }


    public async Task<VerificationAttemptDto> LogVerificationAttemptAsync(Guid userId, string ipAddress, string? deviceInfo)
    {
        var attempt = new VerificationAttempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        var fraudScore = await CalculateFraudScoreAsync(userId, attempt);
        attempt.FraudScore = fraudScore;

        var logged = await _verificationRepository.LogAttemptAsync(attempt);
        return MapAttemptToDto(logged);
    }

    public async Task<decimal> CalculateFraudScoreAsync(Guid userId, VerificationAttempt attempt)
    {
        decimal score = 0.0m;

        // Check IP reputation
        // Check device info against known devices
        // Check attempt frequency
        var recentAttempts = await _verificationRepository.GetUserAttemptsAsync(userId);
        if (recentAttempts.Count(a => a.CreatedAt > DateTime.UtcNow.AddHours(-1)) > 3)
            score += 0.3m; // High attempt frequency

        // Check for VPN/Proxy
        // Could integrate with external API

        return Math.Min(score, 1.0m);
    }

  public async Task<List<VerificationAttemptDto>> GetFlaggedAttemptsAsync()
{
    var attempts = await _verificationRepository.GetFlaggedAttemptsAsync(0.7m);
    return attempts.Select(MapAttemptToDto).ToList();
}



    public async Task<bool> IsAttemptSuspiciousAsync(Guid userId, VerificationAttempt attempt)
    {
        var fraudScore = attempt.FraudScore ?? await CalculateFraudScoreAsync(userId, attempt);
        return fraudScore > 0.7m;
    }

    public async Task<BackgroundCheckResultDto> RequestBackgroundCheckAsync(Guid userId, RequestBackgroundCheckDto request)
    {
        var existingCheck = await _verificationRepository.GetLatestBackgroundCheckAsync(userId);
        if (existingCheck?.ExpiresAt > DateTime.UtcNow)
        {
            // Return existing if still valid
            return MapBackgroundCheckToDto(existingCheck);
        }

        var backgroundCheck = new BackgroundCheckResult
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = "pending",
            ProviderName = "DefaultProvider",
            ProviderReferenceId = Guid.NewGuid().ToString(),
            RequestedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddYears(1)
        };

        var created = await _verificationRepository.CreateBackgroundCheckAsync(backgroundCheck);
        
        // Log action
        await _verificationRepository.LogActionAsync(new VerificationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = "background_check_requested",
            Details = "Background check requested",
            CreatedAt = DateTime.UtcNow
        });

        return MapBackgroundCheckToDto(created);
    }

    public async Task<BackgroundCheckResultDto?> GetBackgroundCheckStatusAsync(Guid userId)
    {
        var check = await _verificationRepository.GetLatestBackgroundCheckAsync(userId);
        return check != null ? MapBackgroundCheckToDto(check) : null;
    }

    public async Task<bool> IsBackgroundClearAsync(Guid userId)
    {
        var check = await _verificationRepository.GetLatestBackgroundCheckAsync(userId);
        return check != null && check.Status == "clear" && check.ExpiresAt > DateTime.UtcNow;
    }

    public async Task<List<VerificationTimelineEventDto>> GetVerificationTimelineAsync(Guid userId)
    {
        var logs = await _verificationRepository.GetUserVerificationLogAsync(userId);
        return logs.Select(l => new VerificationTimelineEventDto
        {
            Id = l.Id,
            Action = l.Action,
            Details = l.Details,
            AdministratorNotes = l.AdministratorNotes,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<VerificationProgressDto> GetVerificationProgressAsync(Guid userId)
    {
        var status = await GetVerificationStatusAsync(userId);
        
        var steps = new List<VerificationStepDto>
        {
            new()
            {
                StepName = "ID Verification",
                Status = status.IsIdVerified ? "completed" : "pending",
                Description = "Verify your identity with a government ID",
                CompletedAt = status.IsIdVerified ? DateTime.UtcNow : null
            },
            new()
            {
                StepName = "Selfie Verification",
                Status = status.IsPhotoVerified ? "completed" : "pending",
                Description = "Take a selfie for liveness verification",
                CompletedAt = status.IsPhotoVerified ? DateTime.UtcNow : null
            },
            new()
            {
                StepName = "Background Check",
                Status = status.IsBackgroundClear ? "completed" : "pending",
                Description = "Complete background check",
                CompletedAt = status.IsBackgroundClear ? DateTime.UtcNow : null
            }
        };

        var completedSteps = steps.Count(s => s.Status == "completed");

        return new VerificationProgressDto
        {
            TotalSteps = 3,
            CompletedSteps = completedSteps,
            Steps = steps,
            ProgressPercentage = (completedSteps / 3.0m) * 100,
            NextStep = steps.FirstOrDefault(s => s.Status == "pending")?.StepName ?? "All steps completed"
        };
    }

    private async Task<VerificationStatusDto> GetVerificationStatusByRecordAsync(Guid recordId)
    {
        var record = await _verificationRepository.GetVerificationRecordAsync(recordId);
        if (record == null) throw new KeyNotFoundException();

        var documents = await _verificationRepository.GetVerificationDocumentsAsync(recordId);

        return new VerificationStatusDto
        {
            Id = record.Id,
            UserId = record.UserId,
            VerificationType = record.VerificationType,
            Status = record.Status,
            AttemptCount = record.AttemptCount,
            SubmittedAt = record.SubmittedAt,
            ReviewedAt = record.ReviewedAt,
            RejectionReason = record.RejectionReason,
            ExpiresAt = record.ExpiresAt,
            VerificationScore = record.VerificationScore,
            Documents = documents.Select(d => new VerificationDocumentDto
            {
                Id = d.Id,
                DocumentType = d.DocumentType,
                StorageUrl = d.StorageUrl,
                MimeType = d.MimeType,
                FileSizeBytes = d.FileSizeBytes,
                IsProcessed = d.IsProcessed,
                ProcessingStatus = d.ProcessingStatus
            }).ToList()
        };
    }

    private VerificationStatusDto MapRecordToDto(VerificationRecord record)
    {
        return new VerificationStatusDto
        {
            Id = record.Id,
            UserId = record.UserId,
            VerificationType = record.VerificationType,
            Status = record.Status,
            AttemptCount = record.AttemptCount,
            SubmittedAt = record.SubmittedAt,
            ReviewedAt = record.ReviewedAt,
            RejectionReason = record.RejectionReason,
            ExpiresAt = record.ExpiresAt,
            VerificationScore = record.VerificationScore
        };
    }

    private VerificationAttemptDto MapAttemptToDto(VerificationAttempt attempt)
    {
        return new VerificationAttemptDto
        {
            Id = attempt.Id,
            AttemptNumber = attempt.AttemptNumber,
            IpAddress = attempt.IpAddress,
            DeviceInfo = attempt.DeviceInfo,
            Status = attempt.Status,
            FlagReason = attempt.FlagReason,
            FraudScore = attempt.FraudScore,
            CreatedAt = attempt.CreatedAt
        };
    }

    private BackgroundCheckResultDto MapBackgroundCheckToDto(BackgroundCheckResult result)
    {
        return new BackgroundCheckResultDto
        {
            Id = result.Id,
            Status = result.Status,
            CompletedAt = result.CompletedAt,
            Summary = result.Summary,
            HasCriminalRecord = result.HasCriminalRecord,
            HasSexOffenderRecord = result.HasSexOffenderRecord,
            ExpiresAt = result.ExpiresAt
        };
    }
}

/// <summary>
/// Badge service implementation
/// </summary>
public class BadgeService : IBadgeService
{
    private readonly IVerificationRepository _verificationRepository;

    public BadgeService(IVerificationRepository verificationRepository)
    {
        _verificationRepository = verificationRepository;
    }

    public async Task<BadgeDto> AwardBadgeAsync(Guid userId, string badgeType, string reason, DateTime? expiresAt = null)
    {
        if (await UserHasBadgeAsync(userId, badgeType))
            return new BadgeDto(); // Already has badge

        var badge = new UserVerificationBadge
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BadgeType = badgeType,
            DisplayName = GetBadgeDisplayName(badgeType),
            BadgeIcon = GetBadgeIcon(badgeType),
            Description = GetBadgeDescription(badgeType),
            Priority = GetBadgePriority(badgeType),
            IsActive = true,
            AwardedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            Criteria = reason
        };

        var awarded = await _verificationRepository.AwardBadgeAsync(badge);
        return MapBadgeToDto(awarded);
    }

    public async Task<bool> RemoveBadgeAsync(Guid userId, string badgeType)
    {
        var badges = await _verificationRepository.GetUserBadgesAsync(userId);
        var badge = badges.FirstOrDefault(b => b.BadgeType == badgeType);
        
        if (badge == null) return false;
        
        return await _verificationRepository.RemoveBadgeAsync(badge.Id);
    }

    public async Task<List<BadgeDto>> GetUserBadgesAsync(Guid userId)
    {
        var badges = await _verificationRepository.GetUserBadgesAsync(userId);
        return badges.Select(MapBadgeToDto).ToList();
    }

    public async Task<bool> UserHasBadgeAsync(Guid userId, string badgeType)
    {
        return await _verificationRepository.UserHasBadgeAsync(userId, badgeType);
    }

    public async Task AwardVerificationBadgeAsync(Guid userId)
    {
        await AwardBadgeAsync(userId, "verified", "User passed all verification checks");
    }

    public async Task AwardBackgroundClearBadgeAsync(Guid userId)
    {
        await AwardBadgeAsync(userId, "background_clear", "User passed background check", DateTime.UtcNow.AddYears(1));
    }

    public async Task AwardCompletionBadgeAsync(Guid userId)
    {
        await AwardBadgeAsync(userId, "profile_complete", "User has a complete profile (>90% completion)");
    }

    public async Task AwardCommunityBadgeAsync(Guid userId)
    {
        await AwardBadgeAsync(userId, "community_contributor", "Active community member");
    }

    public async Task<List<BadgeDto>> GetExpiringBadgesAsync()
    {
        var threshold = DateTime.UtcNow.AddDays(7);

        var badges = await _verificationRepository.GetBadgesExpiringBeforeAsync(threshold);

        return badges.Select(MapBadgeToDto).ToList();
    }


    public async Task ExpireBadgeAsync(Guid badgeId)
    {
        var badge = await _verificationRepository.GetBadgeAsync(badgeId);
        if (badge == null) return;

        badge.IsActive = false;
        badge.ExpiresAt = DateTime.UtcNow;

        await _verificationRepository.UpdateBadgeAsync(badge);
    }


    public async Task RenewBadgeAsync(Guid badgeId)
    {
        var badge = await _verificationRepository.GetBadgeAsync(badgeId);
        if (badge == null) return;

        badge.IsActive = true;
        badge.ExpiresAt = DateTime.UtcNow.AddYears(1);

        await _verificationRepository.UpdateBadgeAsync(badge);
    }


    public async Task<VerificationStatsDto> GetBadgeStatsAsync()
{
    var allBadges = await _verificationRepository.GetAllBadgesAsync();
    var allRecords = await _verificationRepository.GetAllVerificationRecordsAsync();
    var allAttempts = await _verificationRepository.GetAllVerificationAttemptsAsync();

    var verifiedUsers = allRecords
        .Where(r => r.Status == "approved")
        .Select(r => r.UserId)
        .Distinct()
        .Count();

    var totalUsersAttempted = allRecords
        .Select(r => r.UserId)
        .Distinct()
        .Count();

    return new VerificationStatsDto
    {
        TotalVerifiedUsers = verifiedUsers,

        IdVerifiedCount = allRecords.Count(r =>
            r.VerificationType == "id" && r.Status == "approved"),

        PhotoVerifiedCount = allRecords.Count(r =>
            r.VerificationType == "photo" && r.Status == "approved"),

        BackgroundClearCount = allRecords.Count(r =>
            r.VerificationType == "background" && r.Status == "approved"),

        VerificationRate = totalUsersAttempted == 0
            ? 0
            : Math.Round((decimal)verifiedUsers / totalUsersAttempted * 100, 2),

        PendingVerifications = allRecords.Count(r => r.Status == "pending"),

        FlaggedAttempts = allAttempts.Count(a => a.FraudScore >= 0.7m)
    };
}



    public async Task<int> GetUserBadgeCountAsync(Guid userId)
    {
        var badges = await _verificationRepository.GetUserBadgesAsync(userId);
        return badges.Count(b => b.IsActive);
    }

    private string GetBadgeDisplayName(string badgeType) => badgeType switch
    {
        "verified" => "Verified",

        "id_verified" => "ID Verified",
        "selfie_verified" => "Selfie Verified",
        "background_clear" => "Background Clear",
        "profile_complete" => "Profile Complete",
        "community_contributor" => "Community Contributor",
        _ => "Badge"
    };

    private string GetBadgeIcon(string badgeType) => badgeType switch
    {
        "verified" => "✓",
        "id_verified" => "🆔",
        "selfie_verified" => "📸",
        "background_clear" => "✅",
        "profile_complete" => "⭐",
        "community_contributor" => "👥",
        _ => "🏆"
    };

    private string GetBadgeDescription(string badgeType) => badgeType switch
    {
        "verified" => "User has passed all verification checks",
        "id_verified" => "User ID has been verified",
        "selfie_verified" => "User verified via selfie",
        "background_clear" => "User has a clear background check",
        "profile_complete" => "User has a complete profile",
        "community_contributor" => "Active community member",
        _ => "User badge"
    };

    private int GetBadgePriority(string badgeType) => badgeType switch
    {
        "background_clear" => 10,
        "id_verified" => 9,
        "verified" => 8,
        "profile_complete" => 5,
        "community_contributor" => 3,
        _ => 1
    };

    private BadgeDto MapBadgeToDto(UserVerificationBadge badge)
    {
        return new BadgeDto
        {
            Id = badge.Id,
            BadgeType = badge.BadgeType,
            DisplayName = badge.DisplayName,
            BadgeIcon = badge.BadgeIcon,
            Description = badge.Description,
            Priority = badge.Priority,
            IsActive = badge.IsActive,
            AwardedAt = badge.AwardedAt,
            ExpiresAt = badge.ExpiresAt
        };
    }
}
