namespace Tindro.Infrastructure.Persistence.Repositories;

using Tindro.Application.Stories.Interfaces;
using Tindro.Domain.Stories;
using Microsoft.EntityFrameworkCore;

public class StoryRepository : IStoryRepository
{
    private readonly QueryDbContext _context;

    public StoryRepository(QueryDbContext context)
    {
        _context = context;
    }

    // CRUD operations
    public async Task<Story?> GetStoryAsync(Guid storyId)
    {
        return await _context.Stories.FirstOrDefaultAsync(s => s.Id == storyId && !s.IsDeleted);
    }

    public async Task<Story?> GetStoryWithDetailsAsync(Guid storyId)
    {
        return await _context.Stories
            .Include(s => s.Likes)
            .Include(s => s.Comments)
            .Include(s => s.Views)
            .FirstOrDefaultAsync(s => s.Id == storyId && !s.IsDeleted);
    }

    public async Task<Story> CreateStoryAsync(Story story)
    {
        _context.Stories.Add(story);
        await _context.SaveChangesAsync();
        return story;
    }

    public async Task<Story> UpdateStoryAsync(Story story)
    {
        _context.Stories.Update(story);
        await _context.SaveChangesAsync();
        return story;
    }

    public async Task DeleteStoryAsync(Guid storyId)
    {
        var story = await GetStoryAsync(storyId);
        if (story != null)
        {
            story.IsDeleted = true;
            story.DeletedAt = DateTime.UtcNow;
            await UpdateStoryAsync(story);
        }
    }

    // User stories
    public async Task<List<Story>> GetUserStoriesAsync(Guid userId, bool includeExpired = false)
    {
        var query = _context.Stories
            .Where(s => s.UserId == userId && !s.IsDeleted);

        if (!includeExpired)
        {
            var expiryTime = DateTime.UtcNow.AddHours(-24);
            query = query.Where(s => s.CreatedAt > expiryTime);
        }

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<List<Story>> GetActiveStoriesAsync(Guid userId)
    {
        var expiryTime = DateTime.UtcNow.AddHours(-24);
        return await _context.Stories
            .Where(s => s.UserId == userId && !s.IsDeleted && s.CreatedAt > expiryTime)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    // Feed
    public async Task<List<Story>> GetFeedStoriesAsync(Guid userId, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var expiryTime = DateTime.UtcNow.AddHours(-24);

        return await _context.Stories
            .Where(s => s.UserId != userId && 
                        !s.IsDeleted && 
                        s.CreatedAt > expiryTime &&
                        (s.VisibilityType == "everyone" || s.IsPublic))
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .Include(s => s.Likes)
            .Include(s => s.Comments)
            .ToListAsync();
    }

    // Likes
    public async Task<StoryLike?> GetLikeAsync(Guid storyId, Guid userId)
    {
        return await _context.StoryLikes
            .FirstOrDefaultAsync(l => l.StoryId == storyId && l.UserId == userId);
    }

    public async Task<List<StoryLike>> GetStoryLikesAsync(Guid storyId)
    {
        return await _context.StoryLikes
            .Where(l => l.StoryId == storyId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<StoryLike> AddLikeAsync(StoryLike like)
    {
        _context.StoryLikes.Add(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task RemoveLikeAsync(Guid likeId)
    {
        var like = await _context.StoryLikes.FindAsync(likeId);
        if (like != null)
        {
            _context.StoryLikes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetLikeCountAsync(Guid storyId)
    {
        return await _context.StoryLikes.CountAsync(l => l.StoryId == storyId);
    }

    // Comments
    public async Task<StoryComment?> GetCommentAsync(Guid commentId)
    {
        return await _context.StoryComments
            .Include(c => c.Replies)
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);
    }

    public async Task<List<StoryComment>> GetStoryCommentsAsync(Guid storyId)
    {
        return await _context.StoryComments
            .Where(c => c.StoryId == storyId && !c.IsDeleted && c.ParentCommentId == null)
            .Include(c => c.Replies.Where(r => !r.IsDeleted))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<StoryComment>> GetCommentRepliesAsync(Guid parentCommentId)
    {
        return await _context.StoryComments
            .Where(c => c.ParentCommentId == parentCommentId && !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<StoryComment> AddCommentAsync(StoryComment comment)
    {
        _context.StoryComments.Add(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<StoryComment> UpdateCommentAsync(StoryComment comment)
    {
        _context.StoryComments.Update(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task DeleteCommentAsync(Guid commentId)
    {
        var comment = await _context.StoryComments.FindAsync(commentId);
        if (comment != null)
        {
            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            _context.StoryComments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCommentCountAsync(Guid storyId)
    {
        return await _context.StoryComments
            .CountAsync(c => c.StoryId == storyId && !c.IsDeleted && c.ParentCommentId == null);
    }

    // Comment likes
    public async Task<StoryCommentLike?> GetCommentLikeAsync(Guid commentId, Guid userId)
    {
        return await _context.StoryCommentLikes
            .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);
    }

    public async Task<StoryCommentLike> AddCommentLikeAsync(StoryCommentLike like)
    {
        _context.StoryCommentLikes.Add(like);
        await _context.SaveChangesAsync();
        return like;
    }

    public async Task RemoveCommentLikeAsync(Guid likeId)
    {
        var like = await _context.StoryCommentLikes.FindAsync(likeId);
        if (like != null)
        {
            _context.StoryCommentLikes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCommentLikeCountAsync(Guid commentId)
    {
        return await _context.StoryCommentLikes.CountAsync(l => l.CommentId == commentId);
    }

    // Views
    public async Task<StoryView?> GetViewAsync(Guid storyId, Guid userId)
    {
        return await _context.StoryViews
            .FirstOrDefaultAsync(v => v.StoryId == storyId && v.UserId == userId);
    }

    public async Task<List<StoryView>> GetStoryViewsAsync(Guid storyId)
    {
        return await _context.StoryViews
            .Where(v => v.StoryId == storyId)
            .OrderByDescending(v => v.ViewedAt)
            .ToListAsync();
    }

    public async Task<List<StoryView>> GetUserViewsForStoryAsync(Guid storyId, int limit)
    {
        return await _context.StoryViews
            .Where(v => v.StoryId == storyId)
            .OrderByDescending(v => v.ViewedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<StoryView> AddViewAsync(StoryView view)
    {
        _context.StoryViews.Add(view);
        await _context.SaveChangesAsync();
        return view;
    }

    public async Task<StoryView> UpdateViewAsync(StoryView view)
    {
        _context.StoryViews.Update(view);
        await _context.SaveChangesAsync();
        return view;
    }

    public async Task<int> GetViewCountAsync(Guid storyId)
    {
        return await _context.StoryViews.CountAsync(v => v.StoryId == storyId);
    }

    public async Task<int> GetUniqueViewerCountAsync(Guid storyId)
    {
        return await _context.StoryViews
            .Where(v => v.StoryId == storyId)
            .Select(v => v.UserId)
            .Distinct()
            .CountAsync();
    }

    // Analytics
    public async Task<StoryAnalytics?> GetAnalyticsAsync(Guid storyId)
    {
        return await _context.StoryAnalytics.FirstOrDefaultAsync(a => a.StoryId == storyId);
    }

    public async Task<StoryAnalytics> UpdateAnalyticsAsync(StoryAnalytics analytics)
    {
        var existing = await _context.StoryAnalytics.FindAsync(analytics.StoryId);
        if (existing != null)
        {
            _context.StoryAnalytics.Update(analytics);
        }
        else
        {
            _context.StoryAnalytics.Add(analytics);
        }
        await _context.SaveChangesAsync();
        return analytics;
    }

    public async Task AggregateAnalyticsAsync(Guid storyId)
    {
        var views = await _context.StoryViews.Where(v => v.StoryId == storyId).ToListAsync();
        var likes = await _context.StoryLikes.Where(l => l.StoryId == storyId).CountAsync();

        var analytics = await _context.StoryAnalytics.FirstOrDefaultAsync(a => a.StoryId == storyId)
            ?? new StoryAnalytics { StoryId = storyId };

        analytics.TotalImpressions = views.Count;
        analytics.UniqueViewers = views.Select(v => v.UserId).Distinct().Count();
        analytics.AverageWatchTimeSeconds = views.Count > 0 ? (int)views.Average(v => v.WatchTimeSeconds) : 0;
        analytics.AverageWatchPercentage = views.Count > 0 ? (int)views.Average(v => v.WatchPercentage) : 0;
        analytics.UpdatedAt = DateTime.UtcNow;

        await UpdateAnalyticsAsync(analytics);
    }

    public async Task<List<Story>> SearchStoriesAsync(string query, Guid userId, int limit = 20)
    {
        var expiryTime = DateTime.UtcNow.AddHours(-24);
        return await _context.Stories
            .Where(s => !s.IsDeleted && 
                        s.CreatedAt > expiryTime &&
                        (s.Caption!.Contains(query) || s.TextContent!.Contains(query)))
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}
