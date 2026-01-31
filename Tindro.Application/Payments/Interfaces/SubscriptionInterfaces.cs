namespace Tindro.Application.Payments.Interfaces;

using Tindro.Application.Payments.Dtos;
using Tindro.Domain.Payments;

/// <summary>
/// Subscription repository interface
/// </summary>
public interface ISubscriptionRepository
{
    // Plans
    Task<SubscriptionPlan?> GetPlanAsync(Guid planId);
    Task<List<SubscriptionPlan>> GetAllPlansAsync();
    Task<SubscriptionPlan> CreatePlanAsync(SubscriptionPlan plan);
    Task UpdatePlanAsync(SubscriptionPlan plan);

    // User subscriptions
    Task<UserSubscription?> GetUserSubscriptionAsync(Guid userId);
    Task<UserSubscription> CreateUserSubscriptionAsync(UserSubscription subscription);
    Task<UserSubscription> UpdateUserSubscriptionAsync(UserSubscription subscription);
    Task CancelSubscriptionAsync(Guid subscriptionId);
    
    // Transactions
    Task<SubscriptionTransaction> CreateTransactionAsync(SubscriptionTransaction transaction);
    Task<List<SubscriptionTransaction>> GetUserTransactionsAsync(Guid userId, int limit = 50);

    // Super likes
    Task<SuperLike?> GetSuperLikeAsync(Guid superLikeId);
    Task<SuperLike> CreateSuperLikeAsync(SuperLike superLike);
    Task<List<SuperLike>> GetReceivedSuperLikesAsync(Guid userId);
    Task MarkSuperLikeViewedAsync(Guid superLikeId);

    // Boosts
    Task<ProfileBoost?> GetActiveBoostAsync(Guid userId);
    Task<ProfileBoost> CreateBoostAsync(ProfileBoost boost);
    Task<List<ProfileBoost>> GetUserBoostHistoryAsync(Guid userId);

    // Permanent posts
    Task<PermanentPost?> GetPermanentPostAsync(Guid postId);
    Task<List<PermanentPost>> GetUserPermanentPostsAsync(Guid userId);
    Task<PermanentPost> CreatePermanentPostAsync(PermanentPost post);
    Task UpdatePermanentPostAsync(PermanentPost post);
    Task DeletePermanentPostAsync(Guid postId);
}

/// <summary>
/// Subscription service interface
/// </summary>
public interface ISubscriptionService
{
    // Plans
    Task<List<SubscriptionPlanDto>> GetAllPlansAsync();
    Task<SubscriptionPlanDto> GetPlanAsync(Guid planId);

    // User subscriptions
    Task<UserSubscriptionDto> GetUserSubscriptionAsync(Guid userId);
    Task<UserSubscriptionDto> SubscribeToPlanAsync(Guid userId, SubscribeToPlanRequestDto request);
    Task CancelSubscriptionAsync(Guid userId);
    Task UpdateAutoRenewAsync(Guid userId, bool autoRenew);
    Task RenewSubscriptionAsync(Guid userId);

    // Subscription status
    Task<bool> IsUserSubscribedAsync(Guid userId);
    Task<bool> HasFeatureAsync(Guid userId, string feature);
    Task<int> GetRemainingQuotaAsync(Guid userId, string featureType);

    // Super likes
    Task<SuperLikeDto> SendSuperLikeAsync(Guid senderId, SendSuperLikeRequestDto request);
    Task<List<SuperLikeDto>> GetReceivedSuperLikesAsync(Guid userId);
    Task MarkSuperLikeViewedAsync(Guid superLikeId);

    // Profile boost
    Task<ProfileBoostDto> StartProfileBoostAsync(Guid userId, StartProfileBoostRequestDto request);
    Task<ProfileBoostDto?> GetActiveBoostAsync(Guid userId);
    Task<List<ProfileBoostDto>> GetBoostHistoryAsync(Guid userId);
    Task IncrementBoostStatsAsync(Guid boostId, string statType, int count = 1);

    // Permanent posts
    Task<PermanentPostDto> CreatePermanentPostAsync(Guid userId, CreatePermanentPostRequestDto request);
    Task<List<PermanentPostDto>> GetUserPermanentPostsAsync(Guid userId);
    Task UpdatePermanentPostAsync(Guid postId, CreatePermanentPostRequestDto request);
    Task DeletePermanentPostAsync(Guid postId);

    // Transactions
    Task<List<SubscriptionTransactionDto>> GetUserTransactionHistoryAsync(Guid userId);
}

/// <summary>
/// Boost & premium feature service interface
/// </summary>
public interface IBoostService
{
    // Quota management
    Task<int> GetSuperLikesRemainingAsync(Guid userId);
    Task<int> GetBoostsRemainingAsync(Guid userId);
    Task<int> GetPermanentPostsRemainingAsync(Guid userId);
    Task DeductQuotaAsync(Guid userId, string featureType, int amount = 1);
    Task ResetWeeklyQuotasAsync();
    Task ResetMonthlyQuotasAsync();

    // Feature availability
    Task<bool> CanSuperLikeAsync(Guid userId);
    Task<bool> CanBoostAsync(Guid userId);
    Task<bool> CanCreatePermanentPostAsync(Guid userId);
    Task<bool> CanSeeWhoLikesAsync(Guid userId);
    Task<bool> CanViewStoryMetricsAsync(Guid userId);
}
