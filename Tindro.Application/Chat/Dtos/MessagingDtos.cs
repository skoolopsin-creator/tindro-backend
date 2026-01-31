using Microsoft.AspNetCore.Http;

namespace Tindro.Application.Chat.Dtos;

/// <summary>
/// Message read receipt DTO
/// </summary>
public class MessageReadReceiptDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public DateTime ReadAt { get; set; }
}

/// <summary>
/// Extended message info with delivery status
/// </summary>
public class ExtendedMessageDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string DeliveryStatus { get; set; } = string.Empty; // "sending", "sent", "delivered", "read", "failed"
    public DateTime? DeliveredAt { get; set; }
    public int ReadByCount { get; set; }
    public List<MessageReadReceiptDto> ReadReceipts { get; set; } = new();
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Typing indicator DTO
/// </summary>
public class TypingIndicatorDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserProfilePicture { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartedAt { get; set; }
}

/// <summary>
/// Start typing request
/// </summary>
public class StartTypingRequestDto
{
    public Guid ConversationId { get; set; }
}

/// <summary>
/// Stop typing request
/// </summary>
public class StopTypingRequestDto
{
    public Guid ConversationId { get; set; }
}

/// <summary>
/// Voice note upload request
/// </summary>
public class UploadVoiceNoteRequestDto
{
    public Guid MessageId { get; set; }

    public IFormFile AudioFile { get; set; } = default!;
    public string AudioUrl { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = "audio/mpeg";
}

/// <summary>
/// Voice note DTO
/// </summary>
public class VoiceNoteDto
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string? TranscribedText { get; set; }
    public bool IsTranscribing { get; set; }
    public int PlayCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Voice note transcription result
/// </summary>
public class VoiceNoteTranscriptionDto
{
    public Guid VoiceNoteId { get; set; }
    public string TranscribedText { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; } // 0-1
}

/// <summary>
/// Mark message as read request
/// </summary>
public class MarkMessageAsReadRequestDto
{
    public Guid MessageId { get; set; }
}

/// <summary>
/// Mark multiple messages as read request
/// </summary>
public class MarkMessagesAsReadRequestDto
{
    public List<Guid> MessageIds { get; set; } = new();
}

/// <summary>
/// Conversation settings DTO
/// </summary>
public class ConversationSettingsDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public bool MuteNotifications { get; set; }
    public bool ShowReadReceipts { get; set; }
    public bool ShowTypingIndicators { get; set; }
    public bool AllowMediaMessages { get; set; }
    public bool AllowVoiceNotes { get; set; }
    public bool AllowCallInvites { get; set; }
}

/// <summary>
/// Update conversation settings request
/// </summary>
public class UpdateConversationSettingsRequestDto
{
    public bool? MuteNotifications { get; set; }
    public bool? ShowReadReceipts { get; set; }
    public bool? ShowTypingIndicators { get; set; }
    public bool? AllowMediaMessages { get; set; }
    public bool? AllowVoiceNotes { get; set; }
    public bool? AllowCallInvites { get; set; }
}

/// <summary>
/// Message info with receipt details
/// </summary>
public class MessageWithReceiptsDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string SenderUsername { get; set; } = string.Empty;
    public string? SenderProfilePicture { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    
    // Receipt info
    public string DeliveryStatus { get; set; } = string.Empty;
    public DateTime? DeliveredAt { get; set; }
    public int ReadByCount { get; set; }
    public List<MessageReadReceiptDto> ReadReceipts { get; set; } = new();
    
    // Media
    public VoiceNoteDto? VoiceNote { get; set; }
    public List<string> MediaUrls { get; set; } = new();
    
    // Edit info
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
}

/// <summary>
/// Conversation info with member typing indicators
/// </summary>
public class ConversationWithTypingDto
{
    public Guid Id { get; set; }
    public List<TypingIndicatorDto> TypingUsers { get; set; } = new();
    public DateTime LastActivityAt { get; set; }
}
