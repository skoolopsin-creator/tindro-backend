namespace Tindro.Application.Stories.Interfaces;

using Tindro.Application.Stories.Dtos;
using Tindro.Domain.Stories;

/// <summary>
/// Story repository interface
/// </summary>
public interface IStoryRepository
{
    // Story CRUD
    Task<Story?> GetStoryAsync(Guid storyId);
    Task<Story?> GetStoryWithDetailsAsync(Guid storyId);
    Task<List<Story>> GetUserStoriesAsync(Guid userId, bool includeExpired = false);
    Task<List<Story>> GetActiveStoriesAsync(Guid userId);
    Task<Story> CreateStoryAsync(Story story);
    Task<Story> UpdateStoryAsync(Story story);
    Task DeleteStoryAsync(Guid storyId);
    Task<List<Story>> GetFeedStoriesAsync(Guid userId, int page = 1, int pageSize = 20);

    // Story Likes
    Task<StoryLike?> GetLikeAsync(Guid storyId, Guid userId);
    Task<List<StoryLike>> GetStoryLikesAsync(Guid storyId);
    Task<StoryLike> AddLikeAsync(StoryLike like);
    Task RemoveLikeAsync(Guid likeId);
    Task<int> GetLikeCountAsync(Guid storyId);

    // Story Comments
    Task<StoryComment?> GetCommentAsync(Guid commentId);
    Task<List<StoryComment>> GetStoryCommentsAsync(Guid storyId);
    Task<List<StoryComment>> GetCommentRepliesAsync(Guid parentCommentId);
    Task<StoryComment> AddCommentAsync(StoryComment comment);
    Task<StoryComment> UpdateCommentAsync(StoryComment comment);
    Task DeleteCommentAsync(Guid commentId);
    Task<int> GetCommentCountAsync(Guid storyId);

    // Comment Likes
    Task<StoryCommentLike?> GetCommentLikeAsync(Guid commentId, Guid userId);
    Task<StoryCommentLike> AddCommentLikeAsync(StoryCommentLike like);
    Task RemoveCommentLikeAsync(Guid likeId);
    Task<int> GetCommentLikeCountAsync(Guid commentId);

    // Story Views
    Task<StoryView?> GetViewAsync(Guid storyId, Guid userId);
    Task<List<StoryView>> GetStoryViewsAsync(Guid storyId);
    Task<List<StoryView>> GetUserViewsForStoryAsync(Guid storyId, int limit = 100);
    Task<StoryView> AddViewAsync(StoryView view);
    Task<StoryView> UpdateViewAsync(StoryView view);
    Task<int> GetViewCountAsync(Guid storyId);
    Task<int> GetUniqueViewerCountAsync(Guid storyId);

    // Analytics
    Task<StoryAnalytics?> GetAnalyticsAsync(Guid storyId);
    Task<StoryAnalytics> UpdateAnalyticsAsync(StoryAnalytics analytics);

   
}

/// <summary>
/// Story service interface
/// </summary>
public interface IStoryService
{
    // Story management
    Task<StoryDto> CreateStoryAsync(Guid userId, CreateStoryRequestDto request);
    Task<StoryDto> GetStoryAsync(Guid storyId, Guid? viewerId = null);
    Task<List<StoryDto>> GetUserStoriesAsync(Guid userId);
    Task<List<StoryDto>> GetActiveStoriesAsync(Guid userId);
    Task UpdateStoryAsync(Guid storyId, CreateStoryRequestDto request);
    Task DeleteStoryAsync(Guid storyId);
    Task PinStoryAsync(Guid storyId);
    Task UnpinStoryAsync(Guid storyId);

    // Feed
    Task<StoryFeedDto> GetStoryFeedAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<StoryFeedDto> GetFollowingStoriesAsync(Guid userId, int page = 1, int pageSize = 20);

    // Likes/Reactions
    Task<StoryLikeDto> LikeStoryAsync(Guid storyId, Guid userId, string reactionType = "like");
    Task UnlikeStoryAsync(Guid storyId, Guid userId);
    Task<List<StoryLikeDto>> GetStoryLikesAsync(Guid storyId);
    Task<bool> UserLikedStoryAsync(Guid storyId, Guid userId);

    // Comments
    Task<StoryCommentDto> AddCommentAsync(Guid storyId, Guid userId, CreateCommentRequestDto request);
    Task<StoryCommentDto> UpdateCommentAsync(Guid commentId, Guid userId, string newContent);
    Task DeleteCommentAsync(Guid commentId, Guid userId);
    Task<List<StoryCommentDto>> GetStoryCommentsAsync(Guid storyId, int limit = 50);
    Task<StoryCommentDto> LikeCommentAsync(Guid commentId, Guid userId);
    Task UnlikeCommentAsync(Guid commentId, Guid userId);

    // Views
    Task<StoryViewDto> RecordViewAsync(Guid storyId, Guid userId, int watchPercentage, int watchTimeSeconds);
    Task<List<StoryViewDto>> GetStoryViewersAsync(Guid storyId, int limit = 100);
    Task<int> GetViewCountAsync(Guid storyId);
    Task<int> GetUniqueViewerCountAsync(Guid storyId);

    // Analytics
    Task<StoryAnalyticsDto> GetAnalyticsAsync(Guid storyId);
    Task<List<StoryInteractionSummaryDto>> GetStoryInteractionSummaryAsync(Guid userId);

    // Interactions
    Task<bool> CanViewStoryAsync(Guid storyId, Guid userId);
    Task<bool> CanCommentOnStoryAsync(Guid storyId);
    Task<bool> CanShareStoryAsync(Guid storyId);
    Task ShareStoryAsync(Guid storyId, Guid userId);

    // Search/Filter
    Task<List<StoryDto>> SearchStoriesAsync(StoryFilterRequestDto filter, Guid userId);
}

/// <summary>
/// Story highlighting service (for archived stories)
/// </summary>
public interface IStoryHighlightService
{
    Task<StoryHighlightDto> CreateHighlightAsync(Guid userId, string title);
    Task<List<StoryHighlightDto>> GetUserHighlightsAsync(Guid userId);
    Task AddStoryToHighlightAsync(Guid highlightId, Guid storyId);
    Task RemoveStoryFromHighlightAsync(Guid highlightId, Guid storyId);
    Task DeleteHighlightAsync(Guid highlightId);
}
