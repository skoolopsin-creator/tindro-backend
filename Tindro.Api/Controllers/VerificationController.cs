using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tindro.Application.Verification.Interfaces;
using Tindro.Application.Verification.Dtos;
using Tindro.Api.Extensions;

namespace Tindro.Api.Controllers;

/// <summary>
/// Verification and safety system controller
/// </summary>
[ApiController]
[Route("api/v1/verification")]
[Authorize]
public class VerificationController : ControllerBase
{
    private readonly IVerificationService _verificationService;
    private readonly IBadgeService _badgeService;

    public VerificationController(IVerificationService verificationService, IBadgeService badgeService)
    {
        _verificationService = verificationService;
        _badgeService = badgeService;
    }

    /// <summary>
    /// Submit verification (ID, phone, email, photo)
    /// </summary>
    [HttpPost("submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubmitVerification([FromBody] SubmitVerificationRequestDto request)
    {
        var userId = User.GetUserId();
        var result = await _verificationService.SubmitVerificationAsync(userId, request);
        return Ok(result);
    }

    /// <summary>
    /// Get user verification status
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationStatus()
    {
        var userId = User.GetUserId();
        var status = await _verificationService.GetVerificationStatusAsync(userId);
        return Ok(status);
    }

    /// <summary>
    /// Check if user is verified
    /// </summary>
    [HttpGet("is-verified")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> IsUserVerified()
    {
        var userId = User.GetUserId();
        var isVerified = await _verificationService.IsUserVerifiedAsync(userId);
        return Ok(new { isVerified });
    }

    /// <summary>
    /// Get verification score (0-100)
    /// </summary>
    [HttpGet("score")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationScore()
    {
        var userId = User.GetUserId();
        var score = await _verificationService.GetVerificationScoreAsync(userId);
        return Ok(new { verificationScore = score });
    }

    /// <summary>
    /// Get verification progress
    /// </summary>
    [HttpGet("progress")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationProgress()
    {
        var userId = User.GetUserId();
        var progress = await _verificationService.GetVerificationProgressAsync(userId);
        return Ok(progress);
    }

    /// <summary>
    /// Upload verification document
    /// </summary>
    [HttpPost("documents/upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadVerificationDocument(
        [FromQuery] Guid recordId,
        [FromQuery] string documentType,
        [FromForm] IFormFile file)
    {
        var userId = User.GetUserId();
        
        using var stream = file.OpenReadStream();
        var result = await _verificationService.UploadVerificationDocumentAsync(
            userId, recordId, stream, documentType, file.ContentType);
        
        return Ok(result);
    }

    /// <summary>
    /// Request background check
    /// </summary>
    [HttpPost("background-check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestBackgroundCheck([FromBody] RequestBackgroundCheckDto request)
    {
        var userId = User.GetUserId();
        var result = await _verificationService.RequestBackgroundCheckAsync(userId, request);
        return Ok(result);
    }

    /// <summary>
    /// Get background check status
    /// </summary>
    [HttpGet("background-check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBackgroundCheckStatus()
    {
        var userId = User.GetUserId();
        var status = await _verificationService.GetBackgroundCheckStatusAsync(userId);
        
        if (status == null)
            return NotFound("No background check found");
        
        return Ok(status);
    }

    /// <summary>
    /// Check if background is clear
    /// </summary>
    [HttpGet("background-check/is-clear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> IsBackgroundClear()
    {
        var userId = User.GetUserId();
        var isClear = await _verificationService.IsBackgroundClearAsync(userId);
        return Ok(new { isBackgroundClear = isClear });
    }

    /// <summary>
    /// Get user badges
    /// </summary>
    [HttpGet("badges")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserBadges()
    {
        var userId = User.GetUserId();
        var badges = await _badgeService.GetUserBadgesAsync(userId);
        return Ok(badges);
    }

    /// <summary>
    /// Get verification timeline
    /// </summary>
    [HttpGet("timeline")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVerificationTimeline()
    {
        var userId = User.GetUserId();
        var timeline = await _verificationService.GetVerificationTimelineAsync(userId);
        return Ok(timeline);
    }

    // Admin endpoints
    /// <summary>
    /// [Admin] Get pending verifications
    /// </summary>
    [HttpGet("admin/pending")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPendingVerifications([FromQuery] int limit = 50)
    {
        var pending = await _verificationService.GetPendingVerificationsAsync(limit);
        return Ok(pending);
    }

    /// <summary>
    /// [Admin] Approve verification
    /// </summary>
    [HttpPost("admin/{recordId}/approve")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveVerification(Guid recordId)
    {
        var adminId = User.GetUserId();
        await _verificationService.ApproveVerificationAsync(recordId, adminId);
        return NoContent();
    }

    /// <summary>
    /// [Admin] Reject verification
    /// </summary>
    [HttpPost("admin/{recordId}/reject")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectVerification(Guid recordId, [FromBody] string reason)
    {
        await _verificationService.RejectVerificationAsync(recordId, reason);
        return NoContent();
    }

    /// <summary>
    /// [Admin] Get flagged verification attempts
    /// </summary>
    [HttpGet("admin/flagged")]
    [Authorize(Roles = "Admin,Moderator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetFlaggedAttempts()
    {
        var flagged = await _verificationService.GetFlaggedAttemptsAsync();
        return Ok(flagged);
    }

    /// <summary>
    /// [Admin] Award badge to user
    /// </summary>
    [HttpPost("admin/badges/award")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AwardBadge([FromBody] AwardBadgeRequestDto request)
    {
        var badge = await _badgeService.AwardBadgeAsync(request.UserId, request.BadgeType, request.Reason ?? "", request.ExpiresAt);
        return Ok(badge);
    }

    /// <summary>
    /// [Admin] Get verification statistics
    /// </summary>
    [HttpGet("admin/stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetVerificationStats()
    {
        var stats = await _badgeService.GetBadgeStatsAsync();
        return Ok(stats);
    }
}
