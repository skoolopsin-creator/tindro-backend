namespace Tindro.Application.Payments.Dtos;

/// <summary>
/// Subscription plan DTO for API responses
/// </summary>
public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public decimal PricePerMonth { get; set; }
    public decimal PriceSixMonths { get; set; }
    public decimal PriceYearly { get; set; }
    
    public bool IsPopular { get; set; }
    public string? BadgeText { get; set; }
    
    public List<string> Features { get; set; } = new();
}

/// <summary>
/// User subscription DTO
/// </summary>
public class UserSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public SubscriptionPlanDto Plan { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool AutoRenew { get; set; }
    
    public int SuperLikesRemaining { get; set; }
    public int BoostsRemaining { get; set; }
    public int PermanentPostsRemaining { get; set; }
    public int PermanentStoriesRemaining { get; set; }
}

/// <summary>
/// Subscribe to plan request
/// </summary>
public class SubscribeToPlanRequestDto
{
    public Guid PlanId { get; set; }
    public int BillingPeriod { get; set; } // 1 = monthly, 6 = six months, 12 = yearly
    public string PaymentMethodId { get; set; } = string.Empty;
}

/// <summary>
/// Super like DTO
/// </summary>
public class SuperLikeDto
{
    public Guid Id { get; set; }
    public Guid SentByUserId { get; set; }
    public string SentByUsername { get; set; } = string.Empty;
    public string? SentByProfilePicture { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsViewed { get; set; }
}

/// <summary>
/// Send super like request
/// </summary>
public class SendSuperLikeRequestDto
{
    public Guid ReceiveUserId { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Profile boost DTO
/// </summary>
public class ProfileBoostDto
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string BoostType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProfileViewsGained { get; set; }
    public int LikesReceived { get; set; }
}

/// <summary>
/// Start profile boost request
/// </summary>
public class StartProfileBoostRequestDto
{
    public int BoostDurationMinutes { get; set; } = 30; // 30, 60, 180
}

/// <summary>
/// Permanent post DTO
/// </summary>
public class PermanentPostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public bool IsPinned { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create permanent post request
/// </summary>
public class CreatePermanentPostRequestDto
{
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public bool IsPinned { get; set; }
}

/// <summary>
/// Subscription transaction DTO
/// </summary>
public class SubscriptionTransactionDto
{
    public Guid Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Subscription management request
/// </summary>
public class ManageSubscriptionRequestDto
{
    public bool AutoRenew { get; set; }
    public string? PaymentMethodId { get; set; }
}
