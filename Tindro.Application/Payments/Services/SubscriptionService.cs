namespace Tindro.Application.Payments.Services;

using Tindro.Application.Payments.Interfaces;
using Tindro.Application.Payments.Dtos;
using Tindro.Domain.Payments;
using System.Text.Json;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _repository;
    private readonly IBoostService _boostService;

    public SubscriptionService(ISubscriptionRepository repository, IBoostService boostService)
    {
        _repository = repository;
        _boostService = boostService;
    }

    // Plans
    public async Task<List<SubscriptionPlanDto>> GetAllPlansAsync()
    {
        var plans = await _repository.GetAllPlansAsync();
        return plans.Select(MapPlanToDto).ToList();
    }

    public async Task<SubscriptionPlanDto> GetPlanAsync(Guid planId)
    {
        var plan = await _repository.GetPlanAsync(planId);
        if (plan == null) throw new KeyNotFoundException("Plan not found");
        return MapPlanToDto(plan);
    }

    // User subscriptions
    public async Task<UserSubscriptionDto> GetUserSubscriptionAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) throw new KeyNotFoundException("No active subscription");
        
        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        var dto = MapSubscriptionToDto(subscription, plan);
        
        // Add remaining quotas
        dto.SuperLikesRemaining = await _boostService.GetSuperLikesRemainingAsync(userId);
        dto.BoostsRemaining = await _boostService.GetBoostsRemainingAsync(userId);
        dto.PermanentPostsRemaining = await _boostService.GetPermanentPostsRemainingAsync(userId);
        
        return dto;
    }

    public async Task<UserSubscriptionDto> SubscribeToPlanAsync(Guid userId, SubscribeToPlanRequestDto request)
    {
        var plan = await _repository.GetPlanAsync(request.PlanId);
        if (plan == null) throw new KeyNotFoundException("Plan not found");

        // Cancel existing subscription if any
        var existing = await _repository.GetUserSubscriptionAsync(userId);
        if (existing != null)
            await _repository.CancelSubscriptionAsync(existing.Id);

        // Create new subscription
        var subscription = new UserSubscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = plan.Id,
            PlanType = plan.Type,
            BillingPeriod = (BillingPeriod)request.BillingPeriod,
            StartDate = DateTime.UtcNow,
            RenewalDate = CalculateRenewalDate(DateTime.UtcNow, (BillingPeriod)request.BillingPeriod),
            Status = "active",
            AutoRenew = true,
            PaymentMethodId = request.PaymentMethodId,
            AmountPaid = GetPrice(plan, (BillingPeriod)request.BillingPeriod),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateUserSubscriptionAsync(subscription);

        // Create transaction record
        var transaction = new SubscriptionTransaction
        {
            Id = Guid.NewGuid(),
            SubscriptionId = created.Id,
            UserId = userId,
            TransactionType = "purchase",
            Amount = subscription.AmountPaid,
            Status = "completed",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateTransactionAsync(transaction);

        return await GetUserSubscriptionAsync(userId);
    }

    public async Task CancelSubscriptionAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) throw new KeyNotFoundException();

        await _repository.CancelSubscriptionAsync(subscription.Id);
    }

    public async Task UpdateAutoRenewAsync(Guid userId, bool autoRenew)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) throw new KeyNotFoundException();

        subscription.AutoRenew = autoRenew;
        await _repository.UpdateUserSubscriptionAsync(subscription);
    }

    public async Task RenewSubscriptionAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) throw new KeyNotFoundException();

        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        if (plan == null) throw new KeyNotFoundException();

        subscription.RenewalDate = CalculateRenewalDate(DateTime.UtcNow, subscription.BillingPeriod);
        subscription.Status = "active";
        await _repository.UpdateUserSubscriptionAsync(subscription);

        // Create renewal transaction
        var transaction = new SubscriptionTransaction
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            UserId = userId,
            TransactionType = "renewal",
            Amount = subscription.AmountPaid,
            Status = "completed",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateTransactionAsync(transaction);
    }

    // Subscription status
    public async Task<bool> IsUserSubscribedAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        return subscription != null && subscription.Status == "active" && subscription.RenewalDate > DateTime.UtcNow;
    }

    public async Task<bool> HasFeatureAsync(Guid userId, string feature)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) return false;

        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        if (plan == null) return false;

        return feature switch
        {
            "unlimited_likes" => plan.UnlimitedLikes == 1,
            "super_likes" => plan.SuperLikesPerWeek > 0,
            "boosts" => plan.BoostsPerMonth > 0,
            "permanent_posts" => plan.PermanentPostsPerMonth > 0,
            "permanent_stories" => plan.PermanentStoriesPerWeek > 0,
            "see_who_likes" => plan.SeeWhoLikes == 1,
            "view_story_metrics" => plan.ViewStoryMetrics == 1,
            "profile_boost" => plan.ProfilVisibilityBoost == 1,
            _ => false
        };
    }

    public async Task<int> GetRemainingQuotaAsync(Guid userId, string featureType)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) return 0;

        return featureType switch
        {
            "super_likes" => Math.Max(0, (await _boostService.GetSuperLikesRemainingAsync(userId))),
            "boosts" => Math.Max(0, (await _boostService.GetBoostsRemainingAsync(userId))),
            "permanent_posts" => Math.Max(0, (await _boostService.GetPermanentPostsRemainingAsync(userId))),
            _ => 0
        };
    }

    // Super likes
    public async Task<SuperLikeDto> SendSuperLikeAsync(Guid senderId, SendSuperLikeRequestDto request)
    {
        if (!await _boostService.CanSuperLikeAsync(senderId))
            throw new InvalidOperationException("No super likes remaining");

        var superLike = new SuperLike
        {
            Id = Guid.NewGuid(),
            SentByUserId = senderId,
            ReceivedByUserId = request.ReceiveUserId,
            Message = request.Message ?? string.Empty,
            IsViewed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateSuperLikeAsync(superLike);
        await _boostService.DeductQuotaAsync(senderId, "super_likes");

        return new SuperLikeDto
        {
            Id = superLike.Id,
            SentByUserId = superLike.SentByUserId,
            Message = superLike.Message,
            CreatedAt = superLike.CreatedAt,
            IsViewed = false
        };
    }

    public async Task<List<SuperLikeDto>> GetReceivedSuperLikesAsync(Guid userId)
    {
        var superLikes = await _repository.GetReceivedSuperLikesAsync(userId);
        return superLikes.Select(sl => new SuperLikeDto
        {
            Id = sl.Id,
            SentByUserId = sl.SentByUserId,
            Message = sl.Message,
            CreatedAt = sl.CreatedAt,
            IsViewed = sl.IsViewed
        }).ToList();
    }

    public async Task MarkSuperLikeViewedAsync(Guid superLikeId)
    {
        await _repository.MarkSuperLikeViewedAsync(superLikeId);
    }

    // Profile boost
    public async Task<ProfileBoostDto> StartProfileBoostAsync(Guid userId, StartProfileBoostRequestDto request)
    {
        if (!await _boostService.CanBoostAsync(userId))
            throw new InvalidOperationException("No boosts remaining");

        var now = DateTime.UtcNow;
        var boost = new ProfileBoost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            StartTime = now,
            EndTime = now.AddMinutes(request.BoostDurationMinutes),
            BoostType = request.BoostDurationMinutes == 30 ? 1 : 
                       request.BoostDurationMinutes == 60 ? 2 : 3,
            IsActive = true,
            CreatedAt = now
        };

        await _repository.CreateBoostAsync(boost);
        await _boostService.DeductQuotaAsync(userId, "boosts");

        return new ProfileBoostDto
        {
            Id = boost.Id,
            StartTime = boost.StartTime,
            EndTime = boost.EndTime,
            BoostType = boost.BoostType == 1 ? "30 minutes" : 
                       boost.BoostType == 2 ? "1 hour" : "3 hours",
            IsActive = true,
            ProfileViewsGained = 0,
            LikesReceived = 0
        };
    }

    public async Task<ProfileBoostDto?> GetActiveBoostAsync(Guid userId)
    {
        var boost = await _repository.GetActiveBoostAsync(userId);
        if (boost == null) return null;

        return new ProfileBoostDto
        {
            Id = boost.Id,
            StartTime = boost.StartTime,
            EndTime = boost.EndTime,
            BoostType = boost.BoostType == 1 ? "30 minutes" : 
                       boost.BoostType == 2 ? "1 hour" : "3 hours",
            IsActive = boost.IsActive && boost.EndTime > DateTime.UtcNow,
            ProfileViewsGained = boost.ProfileViewsGained,
            LikesReceived = boost.LikesReceived
        };
    }

    public async Task<List<ProfileBoostDto>> GetBoostHistoryAsync(Guid userId)
    {
        var boosts = await _repository.GetUserBoostHistoryAsync(userId);
        return boosts.Select(b => new ProfileBoostDto
        {
            Id = b.Id,
            StartTime = b.StartTime,
            EndTime = b.EndTime,
            BoostType = b.BoostType == 1 ? "30 minutes" : 
                       b.BoostType == 2 ? "1 hour" : "3 hours",
            IsActive = b.IsActive && b.EndTime > DateTime.UtcNow,
            ProfileViewsGained = b.ProfileViewsGained,
            LikesReceived = b.LikesReceived
        }).ToList();
    }

    public async Task IncrementBoostStatsAsync(Guid boostId, string statType, int count = 1)
    {
        // Implementation for tracking boost statistics
        // Would update ProfileViewsGained and LikesReceived as they occur
    }

    // Permanent posts
    public async Task<PermanentPostDto> CreatePermanentPostAsync(Guid userId, CreatePermanentPostRequestDto request)
    {
        if (!await _boostService.CanCreatePermanentPostAsync(userId))
            throw new InvalidOperationException("No permanent posts remaining");

        var post = new PermanentPost
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Content = request.Content,
            MediaUrl = request.MediaUrl,
            IsPinned = request.IsPinned,
            ExpiresAt = DateTime.UtcNow.AddDays(365), // 1 year for permanent
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreatePermanentPostAsync(post);
        await _boostService.DeductQuotaAsync(userId, "permanent_posts");

        return MapPostToDto(post);
    }

    public async Task<List<PermanentPostDto>> GetUserPermanentPostsAsync(Guid userId)
    {
        var posts = await _repository.GetUserPermanentPostsAsync(userId);
        return posts.Select(MapPostToDto).ToList();
    }

    public async Task UpdatePermanentPostAsync(Guid postId, CreatePermanentPostRequestDto request)
    {
        var post = await _repository.GetPermanentPostAsync(postId);
        if (post == null) throw new KeyNotFoundException();

        post.Content = request.Content;
        post.MediaUrl = request.MediaUrl;
        post.IsPinned = request.IsPinned;
        post.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdatePermanentPostAsync(post);
    }

    public async Task DeletePermanentPostAsync(Guid postId)
    {
        await _repository.DeletePermanentPostAsync(postId);
    }

    // Transactions
    public async Task<List<SubscriptionTransactionDto>> GetUserTransactionHistoryAsync(Guid userId)
    {
        var transactions = await _repository.GetUserTransactionsAsync(userId);
        return transactions.Select(t => new SubscriptionTransactionDto
        {
            Id = t.Id,
            TransactionType = t.TransactionType,
            Amount = t.Amount,
            Currency = t.Currency,
            Status = t.Status,
            PaymentMethod = t.PaymentMethod,
            CreatedAt = t.CreatedAt
        }).ToList();
    }

    // Helper methods
    private SubscriptionPlanDto MapPlanToDto(SubscriptionPlan plan)
    {
        var features = new List<string>();
        try
        {
            features = JsonSerializer.Deserialize<List<string>>(plan.Features) ?? new();
        }
        catch { }

        return new SubscriptionPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            PlanType = plan.Type.ToString(),
            Description = plan.Description,
            PricePerMonth = plan.PricePerMonth,
            PriceSixMonths = plan.PriceSixMonths,
            PriceYearly = plan.PriceYearly,
            IsPopular = plan.IsPopular,
            BadgeText = plan.BadgeText,
            Features = features
        };
    }

    private UserSubscriptionDto MapSubscriptionToDto(UserSubscription subscription, SubscriptionPlan? plan)
    {
        return new UserSubscriptionDto
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            Plan = plan != null ? MapPlanToDto(plan) : new SubscriptionPlanDto(),
            StartDate = subscription.StartDate,
            RenewalDate = subscription.RenewalDate,
            Status = subscription.Status,
            AutoRenew = subscription.AutoRenew
        };
    }

    private PermanentPostDto MapPostToDto(PermanentPost post)
    {
        return new PermanentPostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            IsPinned = post.IsPinned,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            ShareCount = post.ShareCount,
            CreatedAt = post.CreatedAt
        };
    }

    private DateTime CalculateRenewalDate(DateTime startDate, BillingPeriod period)
    {
        return period switch
        {
            BillingPeriod.Monthly => startDate.AddMonths(1),
            BillingPeriod.SixMonths => startDate.AddMonths(6),
            BillingPeriod.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1)
        };
    }

    private decimal GetPrice(SubscriptionPlan plan, BillingPeriod period)
    {
        return period switch
        {
            BillingPeriod.Monthly => plan.PricePerMonth,
            BillingPeriod.SixMonths => plan.PriceSixMonths,
            BillingPeriod.Yearly => plan.PriceYearly,
            _ => plan.PricePerMonth
        };
    }
}

/// <summary>
/// Boost service implementation
/// </summary>
public class BoostService : IBoostService
{
    private readonly ISubscriptionRepository _repository;

    public BoostService(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> GetSuperLikesRemainingAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null || subscription.Status != "active") return 0;

        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        if (plan == null) return 0;

        return Math.Max(0, plan.SuperLikesPerWeek - subscription.SuperLikesUsedThisWeek);
    }

    public async Task<int> GetBoostsRemainingAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null || subscription.Status != "active") return 0;

        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        if (plan == null) return 0;

        return Math.Max(0, plan.BoostsPerMonth - subscription.BoostsUsedThisMonth);
    }

    public async Task<int> GetPermanentPostsRemainingAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null || subscription.Status != "active") return 0;

        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        if (plan == null) return 0;

        return Math.Max(0, plan.PermanentPostsPerMonth - subscription.PermanentPostsUsedThisMonth);
    }

    public async Task DeductQuotaAsync(Guid userId, string featureType, int amount = 1)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) return;

        if (featureType == "super_likes")
            subscription.SuperLikesUsedThisWeek += amount;
        else if (featureType == "boosts")
            subscription.BoostsUsedThisMonth += amount;
        else if (featureType == "permanent_posts")
            subscription.PermanentPostsUsedThisMonth += amount;

        await _repository.UpdateUserSubscriptionAsync(subscription);
    }

    public async Task ResetWeeklyQuotasAsync()
    {
        // Would be called by background job every week
        // Reset SuperLikesUsedThisWeek and PermanentStoriesUsedThisWeek
    }

    public async Task ResetMonthlyQuotasAsync()
    {
        // Would be called by background job every month
        // Reset BoostsUsedThisMonth and PermanentPostsUsedThisMonth
    }

    public async Task<bool> CanSuperLikeAsync(Guid userId) => await GetSuperLikesRemainingAsync(userId) > 0;
    public async Task<bool> CanBoostAsync(Guid userId) => await GetBoostsRemainingAsync(userId) > 0;
    public async Task<bool> CanCreatePermanentPostAsync(Guid userId) => await GetPermanentPostsRemainingAsync(userId) > 0;

    public async Task<bool> CanSeeWhoLikesAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) return false;
        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        return plan?.SeeWhoLikes == 1;
    }

    public async Task<bool> CanViewStoryMetricsAsync(Guid userId)
    {
        var subscription = await _repository.GetUserSubscriptionAsync(userId);
        if (subscription == null) return false;
        var plan = await _repository.GetPlanAsync(subscription.PlanId);
        return plan?.ViewStoryMetrics == 1;
    }
}
