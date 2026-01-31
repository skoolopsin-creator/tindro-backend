namespace Tindro.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tindro.Api.Extensions;
using Tindro.Application.Chat.Dtos;
using Tindro.Application.Chat.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagingController : ControllerBase
{
    private readonly IMessagingService _messagingService;
    private readonly IReadReceiptService _readReceiptService;
    private readonly ITypingIndicatorService _typingIndicatorService;
    private readonly IVoiceNoteService _voiceNoteService;
    private readonly ILogger<MessagingController> _logger;

    public MessagingController(IMessagingService messagingService, ILogger<MessagingController> logger, IReadReceiptService readReceiptService, ITypingIndicatorService typingIndicatorService, IVoiceNoteService voiceNoteService)
    {
        _messagingService = messagingService;
        _logger = logger;
        _readReceiptService = readReceiptService;   
        _typingIndicatorService = typingIndicatorService;
        _voiceNoteService = voiceNoteService;
    }

    // ===== READ RECEIPTS =====

    /// <summary>
    /// Mark a message as read by the current user
    /// </summary>
    [HttpPost("read-receipts/mark-read")]
    public async Task<IActionResult> MarkMessageAsRead([FromBody] MarkMessageAsReadRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            await _readReceiptService.MarkMessageAsReadAsync(request.MessageId, userId);

            return Ok(new
            {
                success = true,
                count = 1
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message as read");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Mark multiple messages as read
    /// </summary>
    [HttpPost("read-receipts/mark-multiple-read")]
    public async Task<IActionResult> MarkMessagesAsRead([FromBody] MarkMessagesAsReadRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            await _readReceiptService.MarkMessagesAsReadAsync(request.MessageIds, userId);
            
            return Ok(new
            {
                success = true,
              
                count = request.MessageIds.Count

            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get read receipts for a message
    /// </summary>
    [HttpGet("read-receipts/{messageId}")]
    public async Task<IActionResult> GetMessageReceipts([FromRoute] Guid messageId)
    {
        try
        {
            var receipts = await _readReceiptService.GetMessageReceiptsAsync(messageId);
            
            return Ok(new
            {
                success = true,
                data = receipts,
                count = receipts.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message receipts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get unread message count for a conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}/unread-count")]
    public async Task<IActionResult> GetUnreadCount([FromRoute] Guid conversationId)
    {
        try
        {
            var userId = User.GetUserId();
            var unreadCount = await _readReceiptService.GetUnreadCountAsync(conversationId, userId);
            
            return Ok(new
            {
                success = true,
                unreadCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread count");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== TYPING INDICATORS =====

    /// <summary>
    /// Start typing indicator
    /// </summary>
    [HttpPost("typing/start")]
    public async Task<IActionResult> StartTyping([FromBody] StartTypingRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            await _typingIndicatorService.StartTypingAsync(request.ConversationId, userId);
            
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting typing indicator");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Stop typing indicator
    /// </summary>
    [HttpPost("typing/stop")]
    public async Task<IActionResult> StopTyping([FromBody] StopTypingRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            await _typingIndicatorService.StopTypingAsync(request.ConversationId, userId);
            
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping typing indicator");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get users currently typing in a conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}/typing-users")]
    public async Task<IActionResult> GetTypingUsers([FromRoute] Guid conversationId)
    {
        try
        {
            var typingUsers = await _messagingService.GetActiveTypingUsersAsync(conversationId);
            
            return Ok(new
            {
                success = true,
                data = typingUsers,
                count = typingUsers.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving typing users");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== VOICE NOTES =====

    /// <summary>
    /// Upload a voice note
    /// </summary>
    [HttpPost("voice-notes/upload")]
    public async Task<IActionResult> UploadVoiceNote([FromForm] UploadVoiceNoteRequestDto request)
    {
        try
        {
            if (request.AudioFile == null || request.AudioFile.Length == 0)
                return BadRequest(new { error = "Audio file is required" });

            var userId = User.GetUserId();
            var voiceNoteDto = await _messagingService.UploadVoiceNoteAsync(
                request.MessageId,
                userId,
              
                request
            );
            
            return Ok(new
            {
                success = true,
                data = voiceNoteDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading voice note");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get voice note details
    /// </summary>
    [HttpGet("voice-notes/{voiceNoteId}")]
    public async Task<IActionResult> GetVoiceNote([FromRoute] Guid voiceNoteId)
    {
        try
        {
            var voiceNote = await _voiceNoteService.GetVoiceNoteAsync(voiceNoteId);
            
            if (voiceNote == null)
                return NotFound(new { error = "Voice note not found" });
            
            return Ok(new
            {
                success = true,
                data = voiceNote
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving voice note");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Transcribe a voice note
    /// </summary>
    [HttpPost("voice-notes/{voiceNoteId}/transcribe")]
    public async Task<IActionResult> TranscribeVoiceNote([FromRoute] Guid voiceNoteId)
    {
        try
        {
            var transcribedNote = await _messagingService.TranscribeVoiceNoteAsync(voiceNoteId);
            
            if (transcribedNote == null)
                return NotFound(new { error = "Voice note not found" });
            
            return Ok(new
            {
                success = true,
                data = transcribedNote
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transcribing voice note");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Increment voice note play count
    /// </summary>
    [HttpPost("voice-notes/{voiceNoteId}/play")]
    public async Task<IActionResult> IncrementPlayCount([FromRoute] Guid voiceNoteId)
    {
        try
        {
            await _voiceNoteService.IncrementPlayCountAsync(voiceNoteId);
            
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing play count");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== CONVERSATION SETTINGS =====

    /// <summary>
    /// Get conversation settings for the current user
    /// </summary>
    [HttpGet("conversations/{conversationId}/settings")]
    public async Task<IActionResult> GetConversationSettings([FromRoute] Guid conversationId)
    {
        try
        {
            var userId = User.GetUserId();
            var settings = await _messagingService.GetConversationSettingsAsync(conversationId, userId);
            
            return Ok(new
            {
                success = true,
                data = settings
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation settings");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Update conversation settings for the current user
    /// </summary>
    [HttpPut("conversations/{conversationId}/settings")]
    public async Task<IActionResult> UpdateConversationSettings([FromRoute] Guid conversationId, [FromBody] UpdateConversationSettingsRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var settings = await _messagingService.UpdateConversationSettingsAsync(conversationId, userId, request);
            
            return Ok(new
            {
                success = true,
                data = settings
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conversation settings");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== MESSAGE DELIVERY STATUS =====

    /// <summary>
    /// Get message with all read receipts
    /// </summary>
    [HttpGet("messages/{messageId:guid}/with-receipts")]
    public async Task<IActionResult> GetMessageWithReceipts([FromRoute] Guid messageId)
    {
        try
        {
            var userId = User.GetUserId();
            var message = await _messagingService.GetMessageWithReceiptsAsync(messageId, userId);
            
            if (message == null)
                return NotFound(new { error = "Message not found" });
            
            return Ok(new
            {
                success = true,
                data = message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving message with receipts");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get conversation messages with delivery status and receipts
    /// </summary>
    [HttpGet("conversations/{conversationId:guid}/messages-with-receipts")]
    public async Task<IActionResult> GetConversationMessagesWithReceipts([FromRoute] Guid conversationId, [FromQuery] int limit = 50)
    {
        try
        {
            var userId = User.GetUserId();
            var messages = await _messagingService.GetConversationMessagesWithReceiptsAsync(conversationId, userId, limit);
            
            return Ok(new
            {
                success = true,
                data = messages,
                count = messages.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation messages");
            return BadRequest(new { error = ex.Message });
        }
    }

    // ===== MAINTENANCE =====

    /// <summary>
    /// Cleanup expired typing indicators (admin/background job only)
    /// </summary>
    [HttpPost("maintenance/cleanup-typing-indicators")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CleanupExpiredTypingIndicators(CancellationToken ct = default)
    {
        try
        {
              await _messagingService.CleanupExpiredTypingIndicatorsAsync();
            
            return Ok(new
            {
                success = true
               
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up typing indicators");
            return BadRequest(new { error = ex.Message });
        }
    }
}
