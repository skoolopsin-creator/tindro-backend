namespace Tindro.Domain.Stories;

using Tindro.Domain.Users;

/// <summary>
/// User story (similar to Instagram/Snapchat stories)
/// </summary>
public class Story
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string MediaUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public string MediaType { get; set; } = "image"; // "image", "video"
    public int? DurationSeconds { get; set; } = 5; // For videos
    public string? BackgroundColor { get; set; } // For text-only stories
    public string? TextContent { get; set; }
    public string? TextColor { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool IsPublic { get; set; } = true; // Can be set to private
    public string? VisibilityType { get; set; } = "everyone"; // "everyone", "friends", "close_friends", "specific_users"
    public string? AllowedUserIds { get; set; } // JSON serialized list if visibility is specific_users
    public bool AllowComments { get; set; } = true;
    public bool AllowSharing { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;    public DateTime? UpdatedAt { get; set; }    public DateTime ExpiresAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsPinned { get; set; }
    public int Position { get; set; } // For pinned stories order

    // Navigation
    public virtual User? User { get; set; }
    public virtual ICollection<StoryLike>? Likes { get; set; } = new List<StoryLike>();
    public virtual ICollection<StoryComment>? Comments { get; set; } = new List<StoryComment>();
    public virtual ICollection<StoryView>? Views { get; set; } = new List<StoryView>();
}

/// <summary>
/// Story like/reaction
/// </summary>
public class StoryLike
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public string ReactionType { get; set; } = "like"; // "like", "love", "haha", "wow", "sad", "angry"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Story? Story { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Story comment/reply
/// </summary>
public class StoryComment
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ParentCommentId { get; set; } // For nested replies
    public string Content { get; set; } = null!;
    public int LikeCount { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public virtual Story? Story { get; set; }
    public virtual User? User { get; set; }
    public virtual StoryComment? ParentComment { get; set; }
    public virtual ICollection<StoryComment>? Replies { get; set; } = new List<StoryComment>();
    public virtual ICollection<StoryCommentLike>? Likes { get; set; } = new List<StoryCommentLike>();
}

/// <summary>
/// Story comment like
/// </summary>
public class StoryCommentLike
{
    public Guid Id { get; set; }
    public Guid CommentId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual StoryComment? Comment { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Story view tracking
/// </summary>
public class StoryView
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int WatchPercentage { get; set; } // 0-100
    public int WatchTimeSeconds { get; set; }
    public bool IsCompleteView { get; set; } // Watched until end
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Story? Story { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Story analytics and metadata
/// </summary>
public class StoryAnalytics
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public int TotalImpressions { get; set; }
    public int UniqueViewers { get; set; }
    public int AverageWatchTimeSeconds { get; set; }
    public decimal AverageWatchPercentage { get; set; }
    public int ClickThroughCount { get; set; }
    public int ShareCount { get; set; }
    public int SaveCount { get; set; }
    public List<string>? TopViewersLastDay { get; set; } // JSON: Last day viewers
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Story? Story { get; set; }
}
