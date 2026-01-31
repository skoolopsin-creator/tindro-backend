namespace Tindro.Domain.Chat;

using Tindro.Domain.Users;

/// <summary>
/// Message read receipt tracking
/// </summary>
public class MessageReadReceipt
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ReadAt { get; set; }
    
    // Navigation
    public virtual Message? Message { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Typing indicator tracking for real-time UI
/// </summary>
public class TypingIndicator
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? StoppedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation
    public virtual Conversation? Conversation { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Voice note/audio message
/// </summary>
public class VoiceNote
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public Guid UserId { get; set; }
    
    // Audio properties
    public string AudioUrl { get; set; } = null!;
    public string MimeType { get; set; } = "audio/mpeg"; // audio/mpeg, audio/wav, audio/ogg
    public int DurationSeconds { get; set; }
    public long FileSizeBytes { get; set; }
    
    // Transcription
    public string? TranscribedText { get; set; }
    public bool IsTranscribing { get; set; }
    
    // Playback tracking
    public int PlayCount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation
    public virtual Message? Message { get; set; }
    public virtual User? User { get; set; }
}

/// <summary>
/// Message delivery status
/// </summary>
public enum MessageDeliveryStatus
{
    Sending = 0,
    Sent = 1,
    Delivered = 2,
    Read = 3,
    Failed = 4,
}

/// <summary>
/// Extended Message entity with read receipts
/// </summary>
public class MessageExtension
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    
    // Delivery tracking
    public MessageDeliveryStatus DeliveryStatus { get; set; } = MessageDeliveryStatus.Sending;
    public DateTime? DeliveredAt { get; set; }
    public int ReadByCount { get; set; }
    
    // Reactions/Editing
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Metadata
    public string? DeviceInfo { get; set; } // Device type that sent message
    
    // Navigation
    public virtual Message? Message { get; set; }
    public virtual ICollection<MessageReadReceipt> ReadReceipts { get; set; } = new List<MessageReadReceipt>();
}

/// <summary>
/// Conversation settings for messaging preferences
/// </summary>
public class ConversationSettings
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    
    // Notification settings
    public bool MuteNotifications { get; set; }
    public bool ShowReadReceipts { get; set; } = true;
    public bool ShowTypingIndicators { get; set; } = true;
    
    // Privacy
    public bool AllowMediaMessages { get; set; } = true;
    public bool AllowVoiceNotes { get; set; } = true;
    public bool AllowCallInvites { get; set; } = true;
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public virtual Conversation? Conversation { get; set; }
    public virtual User? User { get; set; }
}
