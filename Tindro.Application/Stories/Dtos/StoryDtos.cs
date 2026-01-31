namespace Tindro.Application.Stories.Dtos;

/// <summary>
/// Story DTO for API responses
/// </summary>
public class StoryDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string MediaType { get; set; } = "image";
    public int? DurationSeconds { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextContent { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ShareCount { get; set; }
    public bool AllowComments { get; set; }
    public bool AllowSharing { get; set; }
    public bool IsPinned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool UserLiked { get; set; }
    public string? UserReactionType { get; set; }
    public bool UserViewed { get; set; }
    public int ReplyCount { get; set; }
}

/// <summary>
/// Create/update story request
/// </summary>
public class CreateStoryRequestDto
{
    public string MediaUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string MediaType { get; set; } = "image"; // "image", "video"
    public int? DurationSeconds { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextContent { get; set; }
    public string? TextColor { get; set; }
    public bool AllowComments { get; set; } = true;
    public bool AllowSharing { get; set; } = true;
    public string? VisibilityType { get; set; } = "everyone";
    public List<Guid>? SpecificUserIds { get; set; }
    public bool IsPinned { get; set; }
}

/// <summary>
/// Story like DTO
/// </summary>
public class StoryLikeDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public string ReactionType { get; set; } = "like";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Story comment DTO
/// </summary>
public class StoryCommentDto
{
    public Guid Id { get; set; }
    public Guid StoryId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public string Content { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public bool UserLiked { get; set; }
    public List<StoryCommentDto>? Replies { get; set; } = new();
    public int ReplyCount { get; set; }
    public bool IsEdited { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Create comment request
/// </summary>
public class CreateCommentRequestDto
{
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}

/// <summary>
/// Story view DTO
/// </summary>
public class StoryViewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public int WatchPercentage { get; set; }
    public int WatchTimeSeconds { get; set; }
    public bool IsCompleteView { get; set; }
    public DateTime ViewedAt { get; set; }
}

/// <summary>
/// Story feed item (for feed endpoints)
/// </summary>
public class StoryFeedDto
{
    public List<UserStoryGroupDto> UserStories { get; set; } = new();
}

/// <summary>
/// Grouped stories by user
/// </summary>
public class UserStoryGroupDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public List<StoryDto> Stories { get; set; } = new();
    public bool HasUnviewedStories { get; set; }
    public DateTime LastStoryTime { get; set; }
}

/// <summary>
/// Story analytics DTO
/// </summary>
public class StoryAnalyticsDto
{
    public Guid StoryId { get; set; }
    public int TotalImpressions { get; set; }
    public int UniqueViewers { get; set; }
    public int AverageWatchTimeSeconds { get; set; }
    public decimal AverageWatchPercentage { get; set; }
    public int ClickThroughCount { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public List<string>? ReactionCounts { get; set; } // JSON: reaction breakdown
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Story viewers list DTO (who viewed the story)
/// </summary>
public class StoryViewersDto
{
    public Guid StoryId { get; set; }
    public int TotalViews { get; set; }
    public List<StoryViewDto> Viewers { get; set; } = new();
}

/// <summary>
/// Like story request
/// </summary>
public class LikeStoryRequestDto
{
    public string ReactionType { get; set; } = "like"; // "like", "love", "haha", "wow", "sad", "angry"
}

/// <summary>
/// Story search/filter request
/// </summary>
public class StoryFilterRequestDto
{
    public string? Query { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? VisibilityType { get; set; }
    public bool? OnlyPinned { get; set; }
    public string? MediaType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Story interaction summary (for notifications)
/// </summary>
public class StoryInteractionSummaryDto
{
    public Guid StoryId { get; set; }
    public int NewLikes { get; set; }
    public int NewComments { get; set; }
    public int NewViews { get; set; }
    public List<string> RecentInteractors { get; set; } = new(); // Usernames
    public DateTime LastInteractionTime { get; set; }
}

/// <summary>
/// Story highlight DTO (archived stories)
/// </summary>
public class StoryHighlightDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int StoryCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
