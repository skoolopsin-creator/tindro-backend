namespace Tindro.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Stories.Dtos;
using Tindro.Application.Stories.Interfaces;
using Tindro.Api.Extensions;

/// <summary>
/// Story API endpoints for creating, viewing, interacting with stories
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoriesController : ControllerBase
{
    private readonly IStoryService _storyService;
    private readonly ILogger<StoriesController> _logger;

    public StoriesController(IStoryService storyService, ILogger<StoriesController> logger)
    {
        _storyService = storyService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new story
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoryDto>> CreateStory([FromBody] CreateStoryRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var story = await _storyService.CreateStoryAsync(userId, request);
            return CreatedAtAction(nameof(GetStory), new { id = story.Id }, story);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating story");
            return BadRequest("Failed to create story");
        }
    }

    /// <summary>
    /// Get a specific story by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoryDto>> GetStory(Guid id)
    {
        try
        {
            var viewerId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (Guid?)null;
            var story = await _storyService.GetStoryAsync(id, viewerId);
            return Ok(story);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid("Cannot access this story");
        }
    }

    /// <summary>
    /// Get all stories for current user
    /// </summary>
    [HttpGet("user/all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryDto>>> GetUserStories()
    {
        var userId = User.GetUserId();
        var stories = await _storyService.GetUserStoriesAsync(userId);
        return Ok(stories);
    }

    /// <summary>
    /// Get active stories (within 24 hours) for current user
    /// </summary>
    [HttpGet("user/active")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryDto>>> GetActiveStories()
    {
        var userId = User.GetUserId();
        var stories = await _storyService.GetActiveStoriesAsync(userId);
        return Ok(stories);
    }

    /// <summary>
    /// Get story feed with all stories visible to current user
    /// </summary>
    [HttpGet("feed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StoryFeedDto>> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var feed = await _storyService.GetStoryFeedAsync(userId, page, pageSize);
        return Ok(feed);
    }

    /// <summary>
    /// Get stories from users being followed
    /// </summary>
    [HttpGet("feed/following")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StoryFeedDto>> GetFollowingFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var feed = await _storyService.GetFollowingStoriesAsync(userId, page, pageSize);
        return Ok(feed);
    }

    /// <summary>
    /// Update a story
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateStory(Guid id, [FromBody] CreateStoryRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var story = await _storyService.GetStoryAsync(id);
            
            if (story?.UserId != userId)
                return Unauthorized("Cannot update other users' stories");

            await _storyService.UpdateStoryAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
    }

    /// <summary>
    /// Delete a story
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStory(Guid id)
    {
        try
        {
            await _storyService.DeleteStoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
    }

    /// <summary>
    /// Pin a story to top of profile
    /// </summary>
    [HttpPost("{id}/pin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PinStory(Guid id)
    {
        try
        {
            await _storyService.PinStoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
    }

    /// <summary>
    /// Unpin a story
    /// </summary>
    [HttpPost("{id}/unpin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnpinStory(Guid id)
    {
        try
        {
            await _storyService.UnpinStoryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
    }

    // ===== Likes/Reactions =====

    /// <summary>
    /// Like or react to a story
    /// </summary>
    [HttpPost("{storyId}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoryLikeDto>> LikeStory(Guid storyId, [FromBody] LikeStoryRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var like = await _storyService.LikeStoryAsync(storyId, userId, request.ReactionType ?? "like");
            return Ok(like);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Story not found");
        }
    }

    /// <summary>
    /// Unlike a story
    /// </summary>
    [HttpPost("{storyId}/unlike")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlikeStory(Guid storyId)
    {
        var userId = User.GetUserId();
        await _storyService.UnlikeStoryAsync(storyId, userId);
        return NoContent();
    }

    /// <summary>
    /// Get all likes for a story
    /// </summary>
    [HttpGet("{storyId}/likes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryLikeDto>>> GetStoryLikes(Guid storyId)
    {
        var likes = await _storyService.GetStoryLikesAsync(storyId);
        return Ok(likes);
    }

    // ===== Comments =====

    /// <summary>
    /// Add a comment to a story
    /// </summary>
    [HttpPost("{storyId}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoryCommentDto>> AddComment(Guid storyId, [FromBody] CreateCommentRequestDto request)
    {
        try
        {
            var canComment = await _storyService.CanCommentOnStoryAsync(storyId);
            if (!canComment)
                return BadRequest("Comments are disabled for this story");

            var userId = User.GetUserId();
            var comment = await _storyService.AddCommentAsync(storyId, userId, request);
            return CreatedAtAction(nameof(GetComments), new { storyId }, comment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment");
            return BadRequest("Failed to add comment");
        }
    }

    /// <summary>
    /// Get all comments for a story
    /// </summary>
    [HttpGet("{storyId}/comments")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryCommentDto>>> GetComments(Guid storyId, [FromQuery] int limit = 50)
    {
        var comments = await _storyService.GetStoryCommentsAsync(storyId, limit);
        return Ok(comments);
    }

    /// <summary>
    /// Update a comment
    /// </summary>
    [HttpPut("comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StoryCommentDto>> UpdateComment(Guid commentId, [FromBody] CreateCommentRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var comment = await _storyService.UpdateCommentAsync(commentId, userId, request.Content);
            return Ok(comment);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Cannot update this comment");
        }
    }

    /// <summary>
    /// Delete a comment
    /// </summary>
    [HttpDelete("comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        try
        {
            var userId = User.GetUserId();
            await _storyService.DeleteCommentAsync(commentId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    /// <summary>
    /// Like a comment
    /// </summary>
    [HttpPost("comments/{commentId}/like")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StoryCommentDto>> LikeComment(Guid commentId)
    {
        var userId = User.GetUserId();
        var comment = await _storyService.LikeCommentAsync(commentId, userId);
        return Ok(comment);
    }

    /// <summary>
    /// Unlike a comment
    /// </summary>
    [HttpPost("comments/{commentId}/unlike")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UnlikeComment(Guid commentId)
    {
        var userId = User.GetUserId();
        await _storyService.UnlikeCommentAsync(commentId, userId);
        return NoContent();
    }

    // ===== Views/Analytics =====

    /// <summary>
    /// Record a view/watch of a story
    /// </summary>
    [HttpPost("{storyId}/view")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StoryViewDto>> RecordView(Guid storyId, [FromBody] StoryViewDto request)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : Guid.NewGuid();
        var view = await _storyService.RecordViewAsync(storyId, userId, request.WatchPercentage, request.WatchTimeSeconds);
        return Ok(view);
    }

    /// <summary>
    /// Get list of users who viewed a story
    /// </summary>
    [HttpGet("{storyId}/viewers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryViewDto>>> GetViewers(Guid storyId, [FromQuery] int limit = 100)
    {
        var viewers = await _storyService.GetStoryViewersAsync(storyId, limit);
        return Ok(viewers);
    }

    /// <summary>
    /// Get view count for a story
    /// </summary>
    [HttpGet("{storyId}/view-count")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> GetViewCount(Guid storyId)
    {
        var viewCount = await _storyService.GetViewCountAsync(storyId);
        var uniqueViewers = await _storyService.GetUniqueViewerCountAsync(storyId);
        return Ok(new { totalViews = viewCount, uniqueViewers });
    }

    /// <summary>
    /// Get analytics for a story
    /// </summary>
    [HttpGet("{storyId}/analytics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<StoryAnalyticsDto>> GetAnalytics(Guid storyId)
    {
        var analytics = await _storyService.GetAnalyticsAsync(storyId);
        return Ok(analytics);
    }

    /// <summary>
    /// Get interaction summary for current user's stories
    /// </summary>
    [HttpGet("interactions/summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryInteractionSummaryDto>>> GetInteractionSummary()
    {
        var userId = User.GetUserId();
        var summary = await _storyService.GetStoryInteractionSummaryAsync(userId);
        return Ok(summary);
    }

    // ===== Share =====

    /// <summary>
    /// Share a story to another user or platform
    /// </summary>
    [HttpPost("{storyId}/share")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ShareStory(Guid storyId)
    {
        try
        {
            var canShare = await _storyService.CanShareStoryAsync(storyId);
            if (!canShare)
                return BadRequest("Sharing is disabled for this story");

            var userId = User.GetUserId();
            await _storyService.ShareStoryAsync(storyId, userId);
            return Ok(new { message = "Story shared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sharing story");
            return BadRequest("Failed to share story");
        }
    }

    // ===== Search =====

    /// <summary>
    /// Search stories by caption or text content
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StoryDto>>> SearchStories([FromQuery] string query, [FromQuery] int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query cannot be empty");

        var userId = User.Identity?.IsAuthenticated == true ? User.GetUserId() : Guid.NewGuid();
        var filter = new StoryFilterRequestDto { Query = query };
        var results = await _storyService.SearchStoriesAsync(filter, userId);
        return Ok(results.Take(limit));
    }
}
