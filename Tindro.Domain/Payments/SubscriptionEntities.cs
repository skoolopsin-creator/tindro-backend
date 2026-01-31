namespace Tindro.Domain.Payments;

using Tindro.Domain.Users;

/// <summary>
/// Subscription plan types for Tindro premium features
/// </summary>
public enum SubscriptionPlanType
{
    Free = 0,
    Plus = 1,      // TINDRO+ 
    Gold = 2,      // TINDRO Gold
}

/// <summary>
/// Subscription billing period
/// </summary>
public enum BillingPeriod
{
    Monthly = 1,
    SixMonths = 6,
    Yearly = 12,
}

/// <summary>
/// Subscription plan definition
/// </summary>
public class SubscriptionPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!; // "TINDRO+", "TINDRO Gold"
    public SubscriptionPlanType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Pricing
    public decimal PricePerMonth { get; set; }  // ₹499, ₹799
    public decimal PriceSixMonths { get; set; } // ₹1,999, ₹1,499
    public decimal PriceYearly { get; set; }
    
    // Display
    public bool IsPopular { get; set; }
    public string? BadgeText { get; set; } // "MOST POPULAR IN INDIA"
    
    // Features (JSON serialized list)
    public string Features { get; set; } = "[]"; // JSON array of feature strings
    
    // Limits
    public int UnlimitedLikes { get; set; } = 0; // 0 = limited, 1 = unlimited
    public int SuperLikesPerWeek { get; set; }   // 5, 10
    public int BoostsPerMonth { get; set; }      // 1, 4
    public int PermanentPostsPerMonth { get; set; }
    public int PermanentPostsPerWeek { get; set; }
    public int PermanentStoriesPerWeek { get; set; }
    public int ViewStoryMetrics { get; set; } = 0; // 0 = no, 1 = yes
    public int SeeWhoLikes { get; set; } = 0;     // 0 = no, 1 = yes
    public int ProfilVisibilityBoost { get; set; } = 0;
    
    // Metadata
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}

/// <summary>
/// User subscription record
/// </summary>
public class UserSubscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    public SubscriptionPlanType PlanType { get; set; }
    public BillingPeriod BillingPeriod { get; set; }
    
    // Dates
    public DateTime StartDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Status
    public string Status { get; set; } = "active"; // "active", "cancelled", "expired", "pending"
    public bool AutoRenew { get; set; } = true;
    
    // Payment
    public string? PaymentMethodId { get; set; }
    public decimal AmountPaid { get; set; }
    public string? TransactionId { get; set; }
    
    // Current usage/quota
    public int SuperLikesUsedThisWeek { get; set; }
    public int BoostsUsedThisMonth { get; set; }
    public int PermanentPostsUsedThisMonth { get; set; }
    public int PermanentStoriesUsedThisWeek { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public virtual User? User { get; set; }
    public virtual SubscriptionPlan? Plan { get; set; }
    public virtual ICollection<SubscriptionTransaction> Transactions { get; set; } = new List<SubscriptionTransaction>();
}

/// <summary>
/// Transaction history for subscriptions
/// </summary>
public class SubscriptionTransaction
{
    public Guid Id { get; set; }
    public Guid SubscriptionId { get; set; }
    public Guid UserId { get; set; }
    
    // Transaction details
    public string TransactionType { get; set; } = "purchase"; // "purchase", "renewal", "refund", "upgrade"
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string Status { get; set; } = "completed"; // "pending", "completed", "failed", "cancelled"
    
    // Payment gateway info
    public string? PaymentGatewayId { get; set; } // Razorpay ID
    public string? PaymentMethod { get; set; }    // "card", "upi", "wallet"
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public string? FailureReason { get; set; }
    
    // Navigation
    public virtual UserSubscription? Subscription { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Boost record for profile visibility
/// </summary>
public class ProfileBoost
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }  // 30 minutes or custom
    public int BoostType { get; set; } = 1; // 1 = 30 min, 2 = 1 hour, 3 = 3 hours
    public bool IsActive { get; set; }
    
    // Stats
    public int ProfileViewsGained { get; set; }
    public int LikesReceived { get; set; }
    public int MatchesCreated { get; set; }
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    
    // Navigation
    public virtual User? User { get; set; }
}

/// <summary>
/// Super like record
/// </summary>
public class SuperLike
{
    public Guid Id { get; set; }
    public Guid SentByUserId { get; set; }
    public Guid ReceivedByUserId { get; set; }
    public string Message { get; set; } = string.Empty; // Optional message with super like
    public bool IsViewed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ViewedAt { get; set; }
    
    // Navigation
    public virtual User? SentByUser { get; set; }
    public virtual User? ReceivedByUser { get; set; }
}

/// <summary>
/// Permanent post (stays visible longer than normal posts)
/// </summary>
public class PermanentPost
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? MediaUrl { get; set; }
    public bool IsPinned { get; set; }
    public DateTime ExpiresAt { get; set; } // Can be null for truly permanent
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int ShareCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public virtual User? User { get; set; }
}
