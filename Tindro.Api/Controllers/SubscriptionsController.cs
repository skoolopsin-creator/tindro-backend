namespace Tindro.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Payments.Dtos;
using Tindro.Application.Payments.Interfaces;
using Tindro.Api.Extensions;

/// <summary>
/// Subscription and premium features API endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IBoostService _boostService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(
        ISubscriptionService subscriptionService,
        IBoostService boostService,
        ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _boostService = boostService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available subscription plans
    /// </summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SubscriptionPlanDto>>> GetPlans()
    {
        var plans = await _subscriptionService.GetAllPlansAsync();
        return Ok(plans);
    }

    /// <summary>
    /// Get a specific subscription plan
    /// </summary>
    [HttpGet("plans/{planId}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubscriptionPlanDto>> GetPlan(Guid planId)
    {
        try
        {
            var plan = await _subscriptionService.GetPlanAsync(planId);
            return Ok(plan);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Plan not found");
        }
    }

    /// <summary>
    /// Get current user's subscription
    /// </summary>
    [HttpGet("current")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserSubscriptionDto>> GetCurrentSubscription()
    {
        try
        {
            var userId = User.GetUserId();
            var subscription = await _subscriptionService.GetUserSubscriptionAsync(userId);
            return Ok(subscription);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("No active subscription");
        }
    }

    /// <summary>
    /// Subscribe to a plan
    /// </summary>
    [HttpPost("subscribe")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserSubscriptionDto>> Subscribe([FromBody] SubscribeToPlanRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var subscription = await _subscriptionService.SubscribeToPlanAsync(userId, request);
            return CreatedAtAction(nameof(GetCurrentSubscription), subscription);
        }
        catch (KeyNotFoundException)
        {
            return BadRequest("Plan not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to plan");
            return BadRequest("Failed to subscribe");
        }
    }

    /// <summary>
    /// Cancel subscription
    /// </summary>
    [HttpPost("cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelSubscription()
    {
        try
        {
            var userId = User.GetUserId();
            await _subscriptionService.CancelSubscriptionAsync(userId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling subscription");
            return BadRequest("Failed to cancel subscription");
        }
    }

    /// <summary>
    /// Update auto-renewal setting
    /// </summary>
    [HttpPut("auto-renew")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAutoRenew([FromBody] ManageSubscriptionRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            await _subscriptionService.UpdateAutoRenewAsync(userId, request.AutoRenew);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating auto-renew");
            return BadRequest("Failed to update auto-renew");
        }
    }

    /// <summary>
    /// Get subscription transaction history
    /// </summary>
    [HttpGet("transactions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SubscriptionTransactionDto>>> GetTransactions()
    {
        var userId = User.GetUserId();
        var transactions = await _subscriptionService.GetUserTransactionHistoryAsync(userId);
        return Ok(transactions);
    }

    // ===== SUPER LIKES =====

    /// <summary>
    /// Send a super like to another user
    /// </summary>
    [HttpPost("super-likes/send")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SuperLikeDto>> SendSuperLike([FromBody] SendSuperLikeRequestDto request)
    {
        try
        {
            if (!await _boostService.CanSuperLikeAsync(User.GetUserId()))
                return BadRequest("No super likes remaining");

            var userId = User.GetUserId();
            var superLike = await _subscriptionService.SendSuperLikeAsync(userId, request);
            return CreatedAtAction(nameof(GetReceivedSuperLikes), superLike);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending super like");
            return BadRequest("Failed to send super like");
        }
    }

    /// <summary>
    /// Get received super likes
    /// </summary>
    [HttpGet("super-likes/received")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SuperLikeDto>>> GetReceivedSuperLikes()
    {
        var userId = User.GetUserId();
        var superLikes = await _subscriptionService.GetReceivedSuperLikesAsync(userId);
        return Ok(superLikes);
    }

    /// <summary>
    /// Mark super like as viewed
    /// </summary>
    [HttpPost("super-likes/{superLikeId}/view")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkSuperLikeViewed(Guid superLikeId)
    {
        await _subscriptionService.MarkSuperLikeViewedAsync(superLikeId);
        return NoContent();
    }

    /// <summary>
    /// Get super likes remaining
    /// </summary>
    [HttpGet("super-likes/remaining")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> GetSuperLikesRemaining()
    {
        var userId = User.GetUserId();
        var remaining = await _boostService.GetSuperLikesRemainingAsync(userId);
        return Ok(new { superLikesRemaining = remaining });
    }

    // ===== PROFILE BOOST =====

    /// <summary>
    /// Start a profile boost
    /// </summary>
    [HttpPost("boosts/start")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProfileBoostDto>> StartBoost([FromBody] StartProfileBoostRequestDto request)
    {
        try
        {
            if (!await _boostService.CanBoostAsync(User.GetUserId()))
                return BadRequest("No boosts remaining");

            var userId = User.GetUserId();
            var boost = await _subscriptionService.StartProfileBoostAsync(userId, request);
            return CreatedAtAction(nameof(GetActiveBoost), boost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting boost");
            return BadRequest("Failed to start boost");
        }
    }

    /// <summary>
    /// Get active boost for current user
    /// </summary>
    [HttpGet("boosts/active")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileBoostDto>> GetActiveBoost()
    {
        var userId = User.GetUserId();
        var boost = await _subscriptionService.GetActiveBoostAsync(userId);
        if (boost == null) return NotFound("No active boost");
        return Ok(boost);
    }

    /// <summary>
    /// Get boost history
    /// </summary>
    [HttpGet("boosts/history")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProfileBoostDto>>> GetBoostHistory()
    {
        var userId = User.GetUserId();
        var history = await _subscriptionService.GetBoostHistoryAsync(userId);
        return Ok(history);
    }

    /// <summary>
    /// Get boosts remaining
    /// </summary>
    [HttpGet("boosts/remaining")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> GetBoostsRemaining()
    {
        var userId = User.GetUserId();
        var remaining = await _boostService.GetBoostsRemainingAsync(userId);
        return Ok(new { boostsRemaining = remaining });
    }

    // ===== PERMANENT POSTS =====

    /// <summary>
    /// Create a permanent post
    /// </summary>
    [HttpPost("posts/permanent")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PermanentPostDto>> CreatePermanentPost([FromBody] CreatePermanentPostRequestDto request)
    {
        try
        {
            if (!await _boostService.CanCreatePermanentPostAsync(User.GetUserId()))
                return BadRequest("No permanent posts remaining");

            var userId = User.GetUserId();
            var post = await _subscriptionService.CreatePermanentPostAsync(userId, request);
            return CreatedAtAction(nameof(GetPermanentPosts), post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating permanent post");
            return BadRequest("Failed to create permanent post");
        }
    }

    /// <summary>
    /// Get current user's permanent posts
    /// </summary>
    [HttpGet("posts/permanent")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PermanentPostDto>>> GetPermanentPosts()
    {
        var userId = User.GetUserId();
        var posts = await _subscriptionService.GetUserPermanentPostsAsync(userId);
        return Ok(posts);
    }

    /// <summary>
    /// Update a permanent post
    /// </summary>
    [HttpPut("posts/permanent/{postId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePermanentPost(Guid postId, [FromBody] CreatePermanentPostRequestDto request)
    {
        try
        {
            await _subscriptionService.UpdatePermanentPostAsync(postId, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Post not found");
        }
    }

    /// <summary>
    /// Delete a permanent post
    /// </summary>
    [HttpDelete("posts/permanent/{postId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeletePermanentPost(Guid postId)
    {
        await _subscriptionService.DeletePermanentPostAsync(postId);
        return NoContent();
    }

    /// <summary>
    /// Get permanent posts remaining quota
    /// </summary>
    [HttpGet("posts/permanent/remaining")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> GetPermanentPostsRemaining()
    {
        var userId = User.GetUserId();
        var remaining = await _boostService.GetPermanentPostsRemainingAsync(userId);
        return Ok(new { permanentPostsRemaining = remaining });
    }

    // ===== FEATURE ACCESS =====

    /// <summary>
    /// Check if user can see who likes them
    /// </summary>
    [HttpGet("features/see-who-likes")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> CanSeeWhoLikes()
    {
        var userId = User.GetUserId();
        var canAccess = await _boostService.CanSeeWhoLikesAsync(userId);
        return Ok(new { canSeeWhoLikes = canAccess });
    }

    /// <summary>
    /// Check if user can view story metrics
    /// </summary>
    [HttpGet("features/story-metrics")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<dynamic>> CanViewStoryMetrics()
    {
        var userId = User.GetUserId();
        var canAccess = await _boostService.CanViewStoryMetricsAsync(userId);
        return Ok(new { canViewStoryMetrics = canAccess });
    }
}
