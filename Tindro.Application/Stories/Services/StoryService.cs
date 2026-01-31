namespace Tindro.Application.Stories.Services;

using Tindro.Application.Stories.Interfaces;
using Tindro.Application.Stories.Dtos;
using Tindro.Domain.Stories;

public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;

    public StoryService(IStoryRepository storyRepository)
    {
        _storyRepository = storyRepository;
    }

    // Story management
    public async Task<StoryDto> CreateStoryAsync(Guid userId, CreateStoryRequestDto request)
    {
        var story = new Story
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MediaUrl = request.MediaUrl,
            Caption = request.Caption,
            MediaType = request.MediaType,
            DurationSeconds = request.DurationSeconds,
            BackgroundColor = request.BackgroundColor,
            TextContent = request.TextContent,
            AllowComments = request.AllowComments,
            AllowSharing = request.AllowSharing,
            VisibilityType = request.VisibilityType,
            IsPinned = request.IsPinned,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        if (request.SpecificUserIds?.Count > 0)
        {
            story.AllowedUserIds = System.Text.Json.JsonSerializer.Serialize(request.SpecificUserIds);
        }

        var created = await _storyRepository.CreateStoryAsync(story);
        return MapToDto(created);
    }

    public async Task<StoryDto> GetStoryAsync(Guid storyId, Guid? viewerId = null)
    {
        var story = await _storyRepository.GetStoryWithDetailsAsync(storyId);
        if (story == null) throw new KeyNotFoundException("Story not found");

        // Check visibility
        if (!await CanViewStoryAsync(storyId, viewerId ?? Guid.Empty))
            throw new UnauthorizedAccessException("Cannot view this story");

        var dto = MapToDto(story);
        
        if (viewerId.HasValue)
        {
            dto.UserLiked = await UserLikedStoryAsync(storyId, viewerId.Value);
            dto.UserViewed = await UserViewedStoryAsync(storyId, viewerId.Value);
        }

        return dto;
    }

    public async Task<List<StoryDto>> GetUserStoriesAsync(Guid userId)
    {
        var stories = await _storyRepository.GetUserStoriesAsync(userId);
        return stories.Select(MapToDto).ToList();
    }

    public async Task<List<StoryDto>> GetActiveStoriesAsync(Guid userId)
    {
        var stories = await _storyRepository.GetActiveStoriesAsync(userId);
        return stories.Select(MapToDto).ToList();
    }

    public async Task UpdateStoryAsync(Guid storyId, CreateStoryRequestDto request)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story == null) throw new KeyNotFoundException();

        story.Caption = request.Caption;
        story.AllowComments = request.AllowComments;
        story.AllowSharing = request.AllowSharing;
        story.TextContent = request.TextContent;
        
        await _storyRepository.UpdateStoryAsync(story);
    }

    public async Task DeleteStoryAsync(Guid storyId)
    {
        await _storyRepository.DeleteStoryAsync(storyId);
    }

    public async Task PinStoryAsync(Guid storyId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story == null) throw new KeyNotFoundException();

        story.IsPinned = true;
        story.Position = 0;
        await _storyRepository.UpdateStoryAsync(story);
    }

    public async Task UnpinStoryAsync(Guid storyId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story == null) throw new KeyNotFoundException();

        story.IsPinned = false;
        await _storyRepository.UpdateStoryAsync(story);
    }

    // Feed
    public async Task<StoryFeedDto> GetStoryFeedAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var stories = await _storyRepository.GetFeedStoriesAsync(userId, page, pageSize);
        return GroupStoriesByUser(stories, userId);
    }

    public async Task<StoryFeedDto> GetFollowingStoriesAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        // In real implementation, would fetch stories from following users
        return new StoryFeedDto();
    }

    // Likes/Reactions
    public async Task<StoryLikeDto> LikeStoryAsync(Guid storyId, Guid userId, string reactionType = "like")
    {
        var existingLike = await _storyRepository.GetLikeAsync(storyId, userId);
        if (existingLike != null)
            await _storyRepository.RemoveLikeAsync(existingLike.Id);

        var like = new StoryLike
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            UserId = userId,
            ReactionType = reactionType,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _storyRepository.AddLikeAsync(like);
        
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story != null)
        {
            story.LikeCount = await _storyRepository.GetLikeCountAsync(storyId);
            await _storyRepository.UpdateStoryAsync(story);
        }

        return new StoryLikeDto
        {
            Id = created.Id,
            UserId = created.UserId,
            ReactionType = created.ReactionType,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task UnlikeStoryAsync(Guid storyId, Guid userId)
    {
        var like = await _storyRepository.GetLikeAsync(storyId, userId);
        if (like == null) return;

        await _storyRepository.RemoveLikeAsync(like.Id);

        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story != null)
        {
            story.LikeCount = await _storyRepository.GetLikeCountAsync(storyId);
            await _storyRepository.UpdateStoryAsync(story);
        }
    }

    public async Task<List<StoryLikeDto>> GetStoryLikesAsync(Guid storyId)
    {
        var likes = await _storyRepository.GetStoryLikesAsync(storyId);
        return likes.Select(l => new StoryLikeDto
        {
            Id = l.Id,
            UserId = l.UserId,
            ReactionType = l.ReactionType,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<bool> UserLikedStoryAsync(Guid storyId, Guid userId)
    {
        var like = await _storyRepository.GetLikeAsync(storyId, userId);
        return like != null;
    }

    // Comments
    public async Task<StoryCommentDto> AddCommentAsync(Guid storyId, Guid userId, CreateCommentRequestDto request)
    {
        var comment = new StoryComment
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            UserId = userId,
            ParentCommentId = request.ParentCommentId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _storyRepository.AddCommentAsync(comment);

        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story != null)
        {
            story.CommentCount = await _storyRepository.GetCommentCountAsync(storyId);
            await _storyRepository.UpdateStoryAsync(story);
        }

        return MapCommentToDto(created);
    }

    public async Task<StoryCommentDto> UpdateCommentAsync(Guid commentId, Guid userId, string newContent)
    {
        var comment = await _storyRepository.GetCommentAsync(commentId);
        if (comment == null || comment.UserId != userId)
            throw new UnauthorizedAccessException("Cannot update this comment");

        comment.Content = newContent;
        comment.IsEdited = true;
        comment.UpdatedAt = DateTime.UtcNow;

        var updated = await _storyRepository.UpdateCommentAsync(comment);
        return MapCommentToDto(updated);
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await _storyRepository.GetCommentAsync(commentId);
        if (comment == null || comment.UserId != userId)
            throw new UnauthorizedAccessException();

        await _storyRepository.DeleteCommentAsync(commentId);

        if (comment.StoryId != Guid.Empty)
        {
            var story = await _storyRepository.GetStoryAsync(comment.StoryId);
            if (story != null)
            {
                story.CommentCount = await _storyRepository.GetCommentCountAsync(comment.StoryId);
                await _storyRepository.UpdateStoryAsync(story);
            }
        }
    }

    public async Task<List<StoryCommentDto>> GetStoryCommentsAsync(Guid storyId, int limit = 50)
    {
        var comments = await _storyRepository.GetStoryCommentsAsync(storyId);
        var dtos = new List<StoryCommentDto>();

        foreach (var comment in comments.Take(limit))
        {
            var dto = MapCommentToDto(comment);
            
            if (comment.Replies?.Count > 0)
            {
                dto.Replies = comment.Replies.Select(MapCommentToDto).ToList();
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<StoryCommentDto> LikeCommentAsync(Guid commentId, Guid userId)
    {
        var existingLike = await _storyRepository.GetCommentLikeAsync(commentId, userId);
        if (existingLike == null)
        {
            var like = new StoryCommentLike
            {
                Id = Guid.NewGuid(),
                CommentId = commentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            await _storyRepository.AddCommentLikeAsync(like);
        }

        var comment = await _storyRepository.GetCommentAsync(commentId);
        if (comment != null)
        {
            comment.LikeCount = await _storyRepository.GetCommentLikeCountAsync(commentId);
            await _storyRepository.UpdateCommentAsync(comment);
        }

        return MapCommentToDto(comment!);
    }

    public async Task UnlikeCommentAsync(Guid commentId, Guid userId)
    {
        var like = await _storyRepository.GetCommentLikeAsync(commentId, userId);
        if (like != null)
        {
            await _storyRepository.RemoveCommentLikeAsync(like.Id);
            
            var comment = await _storyRepository.GetCommentAsync(commentId);
            if (comment != null)
            {
                comment.LikeCount = await _storyRepository.GetCommentLikeCountAsync(commentId);
                await _storyRepository.UpdateCommentAsync(comment);
            }
        }
    }

    // Views
    public async Task<StoryViewDto> RecordViewAsync(Guid storyId, Guid userId, int watchPercentage, int watchTimeSeconds)
    {
        var existingView = await _storyRepository.GetViewAsync(storyId, userId);
        
        if (existingView != null)
        {
            existingView.WatchPercentage = watchPercentage;
            existingView.WatchTimeSeconds = watchTimeSeconds;
            existingView.IsCompleteView = watchPercentage >= 95;
            existingView.ViewedAt = DateTime.UtcNow;
            var updated = await _storyRepository.UpdateViewAsync(existingView);
            return MapViewToDto(updated);
        }

        var view = new StoryView
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            UserId = userId,
            WatchPercentage = watchPercentage,
            WatchTimeSeconds = watchTimeSeconds,
            IsCompleteView = watchPercentage >= 95,
            ViewedAt = DateTime.UtcNow
        };

        var created = await _storyRepository.AddViewAsync(view);

        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story != null)
        {
            story.ViewCount = await _storyRepository.GetViewCountAsync(storyId);
            await _storyRepository.UpdateStoryAsync(story);
        }

        return MapViewToDto(created);
    }

    public async Task<List<StoryViewDto>> GetStoryViewersAsync(Guid storyId, int limit = 100)
    {
        var views = await _storyRepository.GetUserViewsForStoryAsync(storyId, limit);
        return views.Select(MapViewToDto).ToList();
    }

    public async Task<int> GetViewCountAsync(Guid storyId)
    {
        return await _storyRepository.GetViewCountAsync(storyId);
    }

    public async Task<int> GetUniqueViewerCountAsync(Guid storyId)
    {
        return await _storyRepository.GetUniqueViewerCountAsync(storyId);
    }

    // Analytics
    public async Task<StoryAnalyticsDto> GetAnalyticsAsync(Guid storyId)
    {
        var analytics = await _storyRepository.GetAnalyticsAsync(storyId);
        if (analytics == null) throw new KeyNotFoundException();

        return new StoryAnalyticsDto
        {
            StoryId = analytics.StoryId,
            TotalImpressions = analytics.TotalImpressions,
            UniqueViewers = analytics.UniqueViewers,
            AverageWatchTimeSeconds = analytics.AverageWatchTimeSeconds,
            AverageWatchPercentage = analytics.AverageWatchPercentage,
            Likes = 0, // Would be populated from aggregate
            Comments = 0,
            Shares = analytics.ShareCount,
            UpdatedAt = analytics.UpdatedAt
        };
    }

    public async Task<List<StoryInteractionSummaryDto>> GetStoryInteractionSummaryAsync(Guid userId)
    {
        var stories = await _storyRepository.GetUserStoriesAsync(userId);
        return stories.Select(s => new StoryInteractionSummaryDto
        {
            StoryId = s.Id,
            NewLikes = s.LikeCount,
            NewComments = s.CommentCount,
            NewViews = s.ViewCount,
            LastInteractionTime = s.CreatedAt
        }).ToList();
    }

    // Interactions
    public async Task<bool> CanViewStoryAsync(Guid storyId, Guid userId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story == null) return false;

        if (story.IsPublic || story.VisibilityType == "everyone")
            return true;

        if (story.UserId == userId)
            return true;

        // Would check friends list, close friends list, or specific users here
        return false;
    }

    public async Task<bool> CanCommentOnStoryAsync(Guid storyId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        return story?.AllowComments ?? false;
    }

    public async Task<bool> CanShareStoryAsync(Guid storyId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        return story?.AllowSharing ?? false;
    }

    public async Task ShareStoryAsync(Guid storyId, Guid userId)
    {
        var story = await _storyRepository.GetStoryAsync(storyId);
        if (story != null)
        {
            story.ShareCount++;
            await _storyRepository.UpdateStoryAsync(story);
        }
    }

    // Search/Filter
    public async Task<List<StoryDto>> SearchStoriesAsync(StoryFilterRequestDto filter, Guid userId)
    {
        // Placeholder for search implementation
        return new();
    }

    // Helper methods
    private StoryDto MapToDto(Story story)
    {
        return new StoryDto
        {
            Id = story.Id,
            UserId = story.UserId,
            MediaUrl = story.MediaUrl,
            Caption = story.Caption,
            MediaType = story.MediaType,
            DurationSeconds = story.DurationSeconds,
            BackgroundColor = story.BackgroundColor,
            TextContent = story.TextContent,
            ViewCount = story.ViewCount,
            LikeCount = story.LikeCount,
            CommentCount = story.CommentCount,
            ShareCount = story.ShareCount,
            AllowComments = story.AllowComments,
            AllowSharing = story.AllowSharing,
            IsPinned = story.IsPinned,
            CreatedAt = story.CreatedAt,
            ExpiresAt = story.ExpiresAt
        };
    }

    private StoryCommentDto MapCommentToDto(StoryComment comment)
    {
        return new StoryCommentDto
        {
            Id = comment.Id,
            StoryId = comment.StoryId,
            UserId = comment.UserId,
            Content = comment.Content,
            LikeCount = comment.LikeCount,
            ReplyCount = comment.Replies?.Count ?? 0,
            IsEdited = comment.IsEdited,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    private StoryViewDto MapViewToDto(StoryView view)
    {
        return new StoryViewDto
        {
            Id = view.Id,
            UserId = view.UserId,
            WatchPercentage = view.WatchPercentage,
            WatchTimeSeconds = view.WatchTimeSeconds,
            IsCompleteView = view.IsCompleteView,
            ViewedAt = view.ViewedAt
        };
    }

    private StoryFeedDto GroupStoriesByUser(List<Story> stories, Guid userId)
    {
        var grouped = stories
            .GroupBy(s => s.UserId)
            .Select(g => new UserStoryGroupDto
            {
                UserId = g.Key,
                Stories = g.Select(MapToDto).ToList(),
                LastStoryTime = g.Max(s => s.CreatedAt)
            })
            .ToList();

        return new StoryFeedDto { UserStories = grouped };
    }

    private async Task<bool> UserViewedStoryAsync(Guid storyId, Guid userId)
    {
        var view = await _storyRepository.GetViewAsync(storyId, userId);
        return view != null;
    }
}

/// <summary>
/// Story highlight service implementation
/// </summary>
public class StoryHighlightService : IStoryHighlightService
{
    public async Task<StoryHighlightDto> CreateHighlightAsync(Guid userId, string title)
    {
        return new StoryHighlightDto { UserId = userId, Title = title };
    }

    public async Task<List<StoryHighlightDto>> GetUserHighlightsAsync(Guid userId)
    {
        return new();
    }

    public async Task AddStoryToHighlightAsync(Guid highlightId, Guid storyId)
    {
        // Implementation
    }

    public async Task RemoveStoryFromHighlightAsync(Guid highlightId, Guid storyId)
    {
        // Implementation
    }

    public async Task DeleteHighlightAsync(Guid highlightId)
    {
        // Implementation
    }
}
