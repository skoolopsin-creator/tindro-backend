namespace Tindro.Application.Chat.Interfaces;

using Tindro.Application.Chat.Dtos;
using Tindro.Domain.Chat;

/// <summary>
/// Messaging repository interface
/// </summary>
public interface IMessagingRepository
{
    Task<MessageReadReceipt> CreateReceiptAsync(MessageReadReceipt receipt);

    Task<TypingIndicator> CreateTypingIndicatorAsync(TypingIndicator indicator);

    Task UpdateTypingIndicatorAsync(TypingIndicator indicator);

    Task RemoveTypingIndicatorAsync(Guid indicatorId);

    Task<VoiceNote> CreateVoiceNoteAsync(VoiceNote voiceNote);
    Task UpdateVoiceNoteAsync(VoiceNote voiceNote);

    Task<MessageExtension> CreateMessageExtensionAsync(MessageExtension extension);
    Task UpdateMessageExtensionAsync(MessageExtension extension);

    Task<ConversationSettings> CreateConversationSettingsAsync(ConversationSettings settings);
    Task UpdateConversationSettingsAsync(ConversationSettings settings);

    // Read receipts


   
}

public interface IReadReceiptService
{
    Task MarkMessageAsReadAsync(Guid messageId, Guid userId);
    Task MarkMessagesAsReadAsync(List<Guid> messageIds, Guid userId);
    Task<List<MessageReadReceiptDto>> GetMessageReceiptsAsync(Guid messageId);
    Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);
    Task<Dictionary<Guid, int>> GetUnreadCountsByConversationAsync(Guid userId);
}

/// <summary>
/// Typing indicator service interface
/// </summary>
public interface ITypingIndicatorService
{
    Task StartTypingAsync(Guid conversationId, Guid userId);
    Task StopTypingAsync(Guid conversationId, Guid userId);
    Task<List<TypingIndicatorDto>> GetTypingUsersAsync(Guid conversationId);
    Task ClearExpiredTypingIndicatorsAsync();
    Task<bool> IsUserTypingAsync(Guid conversationId, Guid userId);
}

/// <summary>
/// Voice note service interface
/// </summary>
public interface IVoiceNoteService
{
    Task<VoiceNoteDto> UploadVoiceNoteAsync(Guid messageId, Guid userId, UploadVoiceNoteRequestDto request);
    Task<VoiceNoteDto> GetVoiceNoteAsync(Guid voiceNoteId);
    Task<VoiceNoteTranscriptionDto> TranscribeVoiceNoteAsync(Guid voiceNoteId);
    Task<List<VoiceNoteDto>> GetConversationVoiceNotesAsync(Guid conversationId);
    Task IncrementPlayCountAsync(Guid voiceNoteId);
    Task DeleteVoiceNoteAsync(Guid voiceNoteId);
}

/// <summary>
/// Conversation settings service interface
/// </summary>
public interface IConversationSettingsService
{
    Task<ConversationSettingsDto> GetSettingsAsync(Guid conversationId, Guid userId);
    Task<ConversationSettingsDto> UpdateSettingsAsync(Guid conversationId, Guid userId, UpdateConversationSettingsRequestDto request);
    Task<bool> CanSendVoiceNotesAsync(Guid conversationId, Guid userId);
    Task<bool> CanSendMediaAsync(Guid conversationId, Guid userId);
    Task<bool> ShouldShowReadReceiptsAsync(Guid conversationId, Guid userId);
    Task<bool> ShouldShowTypingIndicatorsAsync(Guid conversationId, Guid userId);
}

/// <summary>
/// Messaging service interface (main orchestrator)
/// </summary>
public interface IMessagingService
{
    // Message delivery & receipts
    Task UpdateMessageDeliveryStatusAsync(Guid messageId, string status);
    Task<MessageWithReceiptsDto> GetMessageWithReceiptsAsync(Guid messageId, Guid userId);
    Task<List<MessageWithReceiptsDto>> GetConversationMessagesWithReceiptsAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50);
    
    // Real-time features
    Task HandleUserTypingAsync(Guid conversationId, Guid userId, bool isTyping);
    Task<List<TypingIndicatorDto>> GetActiveTypingUsersAsync(Guid conversationId);
    
    // Voice notes
    Task<VoiceNoteDto> UploadVoiceNoteAsync(Guid messageId, Guid userId, UploadVoiceNoteRequestDto request);
    Task<VoiceNoteTranscriptionDto> TranscribeVoiceNoteAsync(Guid voiceNoteId);
    
    // Settings & preferences
    Task<ConversationSettingsDto> GetConversationSettingsAsync(Guid conversationId, Guid userId);
    Task<ConversationSettingsDto> UpdateConversationSettingsAsync(Guid conversationId, Guid userId, UpdateConversationSettingsRequestDto request);
    
    // Cleanup
    Task CleanupExpiredTypingIndicatorsAsync();
}
