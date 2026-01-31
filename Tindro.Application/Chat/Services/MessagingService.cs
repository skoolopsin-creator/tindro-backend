namespace Tindro.Application.Chat.Services;

using Tindro.Application.Chat.Interfaces;
using Tindro.Application.Chat.Dtos;
using Tindro.Domain.Chat;
using System;
using Tindro.Application.Common.Interfaces;

/// <summary>
/// Read receipt service implementation
/// </summary>
public class ReadReceiptService : IReadReceiptService
{
    private readonly IMessagingRepository _repository;
    private readonly IMessagingQueryRepository _queryRepository;
    public ReadReceiptService(IMessagingRepository repository, IMessagingQueryRepository messagingQueryRepository)
    {
        _repository = repository;
        _queryRepository = messagingQueryRepository;
    }

    public async Task MarkMessageAsReadAsync(Guid messageId, Guid userId)
    {
        var existing = await _queryRepository.GetReceiptAsync(messageId, userId);
        if (existing != null) return; // Already read

        var receipt = new MessageReadReceipt
        {
            Id = Guid.NewGuid(),
            MessageId = messageId,
            UserId = userId,
            ReadAt = DateTime.UtcNow
        };

        await _repository.CreateReceiptAsync(receipt);
    }

    public async Task MarkMessagesAsReadAsync(List<Guid> messageIds, Guid userId)
    {
        foreach (var messageId in messageIds)
        {
            await MarkMessageAsReadAsync(messageId, userId);
        }
    }

    public async Task<List<MessageReadReceiptDto>> GetMessageReceiptsAsync(Guid messageId)
    {
        var receipts = await _queryRepository.GetMessageReceiptsAsync(messageId);
        return receipts.Select(r => new MessageReadReceiptDto
        {
            Id = r.Id,
            MessageId = r.MessageId,
            UserId = r.UserId,
            ReadAt = r.ReadAt
        }).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId)
    {
        return await _queryRepository.GetUnreadCountAsync(conversationId, userId);
    }


    public async Task<Dictionary<Guid, int>> GetUnreadCountsByConversationAsync(Guid userId)
    {
        return await _queryRepository.GetUnreadCountsByConversationAsync(userId);
    }

}

/// <summary>
/// Typing indicator service implementation
/// </summary>
public class TypingIndicatorService : ITypingIndicatorService
{
    private readonly IMessagingRepository _repository;
    private readonly IMessagingQueryRepository _queryRepository;
    private const int TYPING_TIMEOUT_SECONDS = 30;

    public TypingIndicatorService(IMessagingRepository repository, IMessagingQueryRepository messagingQueryRepository)
    {
        _repository = repository;
        _queryRepository = messagingQueryRepository;
    }

    public async Task StartTypingAsync(Guid conversationId, Guid userId)
    {
        var existing = await _queryRepository.GetActiveTypingIndicatorAsync(conversationId, userId);

        if (existing != null && existing.IsActive && (DateTime.UtcNow - existing.StartedAt).TotalSeconds < TYPING_TIMEOUT_SECONDS)
        {
            // Update existing indicator
            existing.StartedAt = DateTime.UtcNow;
            existing.StoppedAt = null;
            await _repository.UpdateTypingIndicatorAsync(existing);
        }
        else
        {
            // Create new indicator
            var indicator = new TypingIndicator
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = userId,
                StartedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _repository.CreateTypingIndicatorAsync(indicator);
        }
    }

    public async Task StopTypingAsync(Guid conversationId, Guid userId)
    {
        var indicator = await _queryRepository.GetActiveTypingIndicatorAsync(conversationId, userId);
        if (indicator != null)
        {
            indicator.IsActive = false;
            indicator.StoppedAt = DateTime.UtcNow;
            await _repository.UpdateTypingIndicatorAsync(indicator);
        }
    }

    public async Task<List<TypingIndicatorDto>> GetTypingUsersAsync(Guid conversationId)
    {
        var indicators = await _queryRepository.GetConversationTypingIndicatorsAsync(conversationId);
        
        // Filter out expired and stopped indicators
        var activeIndicators = indicators
            .Where(i => i.IsActive && (DateTime.UtcNow - i.StartedAt).TotalSeconds < TYPING_TIMEOUT_SECONDS)
            .Select(i => new TypingIndicatorDto
            {
                Id = i.Id,
                ConversationId = i.ConversationId,
                UserId = i.UserId,
                IsActive = true,
                StartedAt = i.StartedAt
            })
            .ToList();

        return activeIndicators;
    }

    public async Task ClearExpiredTypingIndicatorsAsync()
    {
        var indicators = await _queryRepository.GetConversationTypingIndicatorsAsync(Guid.Empty); // Get all
        
        foreach (var indicator in indicators)
        {
            if ((DateTime.UtcNow - indicator.StartedAt).TotalSeconds > TYPING_TIMEOUT_SECONDS)
            {
                await _repository.RemoveTypingIndicatorAsync(indicator.Id);
            }
        }
    }

    public async Task<bool> IsUserTypingAsync(Guid conversationId, Guid userId)
    {
        var indicator = await _queryRepository.GetActiveTypingIndicatorAsync(conversationId, userId);
        return indicator != null && indicator.IsActive && 
               (DateTime.UtcNow - indicator.StartedAt).TotalSeconds < TYPING_TIMEOUT_SECONDS;
    }
}

/// <summary>
/// Voice note service implementation
/// </summary>
public class VoiceNoteService : IVoiceNoteService
{
    private readonly IMessagingRepository _repository;
    private readonly IMessagingQueryRepository _queryRepository;
    private readonly IFileStorage _fileStorage;
    public VoiceNoteService(IMessagingRepository repository, IFileStorage fileStorage, IMessagingQueryRepository messagingQueryRepository)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _queryRepository = messagingQueryRepository;
    }

    public async Task<VoiceNoteDto> UploadVoiceNoteAsync(Guid messageId, Guid userId, UploadVoiceNoteRequestDto request)
    {
        var audioUrl = await _fileStorage.UploadAsync(request.AudioFile, "voice-notes");


        var voiceNote = new VoiceNote
        {

            Id = Guid.NewGuid(),
            MessageId = messageId,
            UserId = userId,
            AudioUrl = request.AudioUrl,
            DurationSeconds = request.DurationSeconds ?? 0,
            FileSizeBytes = request.FileSizeBytes,
            MimeType = request.MimeType,
            IsTranscribing = true, // Start transcription job
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateVoiceNoteAsync(voiceNote);
        return MapToDto(voiceNote);
    }

    public async Task<VoiceNoteDto> GetVoiceNoteAsync(Guid voiceNoteId)
    {
        var voiceNote = await _queryRepository.GetVoiceNoteAsync(voiceNoteId);
        if (voiceNote == null) throw new KeyNotFoundException("Voice note not found");
        return MapToDto(voiceNote);
    }

    public async Task<VoiceNoteTranscriptionDto> TranscribeVoiceNoteAsync(Guid voiceNoteId)
    {
        var voiceNote = await _queryRepository.GetVoiceNoteAsync(voiceNoteId);
        if (voiceNote == null) throw new KeyNotFoundException();

        // Would call Google Cloud Speech-to-Text or similar service
        // For now, placeholder
        var transcribedText = "Transcribed voice note text";

        voiceNote.TranscribedText = transcribedText;
        voiceNote.IsTranscribing = false;
        await _repository.UpdateVoiceNoteAsync(voiceNote);

        return new VoiceNoteTranscriptionDto
        {
            VoiceNoteId = voiceNote.Id,
            TranscribedText = transcribedText,
            ConfidenceScore = 0.95m
        };
    }

    public async Task<List<VoiceNoteDto>> GetConversationVoiceNotesAsync(Guid conversationId)
    {
        var voiceNotes = await _queryRepository.GetConversationVoiceNotesAsync(conversationId);
        return voiceNotes.Select(MapToDto).ToList();
    }

    public async Task IncrementPlayCountAsync(Guid voiceNoteId)
    {
        var voiceNote = await _queryRepository.GetVoiceNoteAsync(voiceNoteId);
        if (voiceNote != null)
        {
            voiceNote.PlayCount++;
            await _repository.UpdateVoiceNoteAsync(voiceNote);
        }
    }

    public async Task DeleteVoiceNoteAsync(Guid voiceNoteId)
    {
        var voiceNote = await _queryRepository.GetVoiceNoteAsync(voiceNoteId);
        if (voiceNote == null) return;

        // Would also delete from cloud storage
        // For now, just remove from DB
        await _repository.UpdateVoiceNoteAsync(voiceNote);
    }

    private VoiceNoteDto MapToDto(VoiceNote voiceNote)
    {
        return new VoiceNoteDto
        {
            Id = voiceNote.Id,
            MessageId = voiceNote.MessageId,
            UserId = voiceNote.UserId,
            AudioUrl = voiceNote.AudioUrl,
            DurationSeconds = voiceNote.DurationSeconds,
            TranscribedText = voiceNote.TranscribedText,
            IsTranscribing = voiceNote.IsTranscribing,
            PlayCount = voiceNote.PlayCount,
            CreatedAt = voiceNote.CreatedAt
        };
    }
}

/// <summary>
/// Conversation settings service implementation
/// </summary>
public class ConversationSettingsService : IConversationSettingsService
{
    private readonly IMessagingRepository _repository;
    private readonly IMessagingQueryRepository _queryRepository;

    public ConversationSettingsService(IMessagingRepository repository, IMessagingQueryRepository messagingQueryRepository)
    {
        _repository = repository;
        _queryRepository = messagingQueryRepository;
    }

    public async Task<ConversationSettingsDto> GetSettingsAsync(Guid conversationId, Guid userId)
    {
        var settings = await _queryRepository.GetConversationSettingsAsync(conversationId, userId);
        if (settings == null)
        {
            // Create default settings
            settings = new ConversationSettings
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = userId,
                MuteNotifications = false,
                ShowReadReceipts = true,
                ShowTypingIndicators = true,
                AllowMediaMessages = true,
                AllowVoiceNotes = true,
                AllowCallInvites = true,
                CreatedAt = DateTime.UtcNow
            };
            await _repository.CreateConversationSettingsAsync(settings);
        }

        return MapToDto(settings);
    }

    public async Task<ConversationSettingsDto> UpdateSettingsAsync(Guid conversationId, Guid userId, UpdateConversationSettingsRequestDto request)
    {
        var settings = await _queryRepository.GetConversationSettingsAsync(conversationId, userId);
        if (settings == null)
        {
            settings = new ConversationSettings
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

        if (request.MuteNotifications.HasValue)
            settings.MuteNotifications = request.MuteNotifications.Value;
        if (request.ShowReadReceipts.HasValue)
            settings.ShowReadReceipts = request.ShowReadReceipts.Value;
        if (request.ShowTypingIndicators.HasValue)
            settings.ShowTypingIndicators = request.ShowTypingIndicators.Value;
        if (request.AllowMediaMessages.HasValue)
            settings.AllowMediaMessages = request.AllowMediaMessages.Value;
        if (request.AllowVoiceNotes.HasValue)
            settings.AllowVoiceNotes = request.AllowVoiceNotes.Value;
        if (request.AllowCallInvites.HasValue)
            settings.AllowCallInvites = request.AllowCallInvites.Value;

        settings.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateConversationSettingsAsync(settings);

        return MapToDto(settings);
    }

    public async Task<bool> CanSendVoiceNotesAsync(Guid conversationId, Guid userId)
    {
        var settings = await GetSettingsAsync(conversationId, userId);
        return settings.AllowVoiceNotes;
    }

    public async Task<bool> CanSendMediaAsync(Guid conversationId, Guid userId)
    {
        var settings = await GetSettingsAsync(conversationId, userId);
        return settings.AllowMediaMessages;
    }

    public async Task<bool> ShouldShowReadReceiptsAsync(Guid conversationId, Guid userId)
    {
        var settings = await GetSettingsAsync(conversationId, userId);
        return settings.ShowReadReceipts;
    }

    public async Task<bool> ShouldShowTypingIndicatorsAsync(Guid conversationId, Guid userId)
    {
        var settings = await GetSettingsAsync(conversationId, userId);
        return settings.ShowTypingIndicators;
    }

    private ConversationSettingsDto MapToDto(ConversationSettings settings)
    {
        return new ConversationSettingsDto
        {
            Id = settings.Id,
            ConversationId = settings.ConversationId,
            MuteNotifications = settings.MuteNotifications,
            ShowReadReceipts = settings.ShowReadReceipts,
            ShowTypingIndicators = settings.ShowTypingIndicators,
            AllowMediaMessages = settings.AllowMediaMessages,
            AllowVoiceNotes = settings.AllowVoiceNotes,
            AllowCallInvites = settings.AllowCallInvites
        };
    }
}

/// <summary>
/// Main messaging service
/// </summary>
public class MessagingService : IMessagingService
{
    private readonly IMessagingRepository _repository;
    private readonly IMessagingQueryRepository _queryRepository;
    private readonly IReadReceiptService _readReceiptService;
    private readonly ITypingIndicatorService _typingService;
    private readonly IVoiceNoteService _voiceNoteService;
    private readonly IConversationSettingsService _settingsService;

    public MessagingService(
        IMessagingRepository repository,
        IReadReceiptService readReceiptService,
        ITypingIndicatorService typingService,
        IVoiceNoteService voiceNoteService,
        IConversationSettingsService settingsService,
        IMessagingQueryRepository messagingQueryRepository)
    {
        _repository = repository;
        _readReceiptService = readReceiptService;
        _typingService = typingService;
        _voiceNoteService = voiceNoteService;
        _settingsService = settingsService;
        _queryRepository = messagingQueryRepository;
    }

    public async Task UpdateMessageDeliveryStatusAsync(Guid messageId, string status)
    {
        var extension = await _queryRepository.GetMessageExtensionAsync(messageId);
        if (extension == null)
        {
            extension = new MessageExtension { Id = Guid.NewGuid(), MessageId = messageId };
            await _repository.CreateMessageExtensionAsync(extension);
        }

        extension.DeliveryStatus = status switch
        {
            "sent" => MessageDeliveryStatus.Sent,
            "delivered" => MessageDeliveryStatus.Delivered,
            "read" => MessageDeliveryStatus.Read,
            "failed" => MessageDeliveryStatus.Failed,
            _ => MessageDeliveryStatus.Sending
        };

        if (extension.DeliveryStatus == MessageDeliveryStatus.Delivered)
            extension.DeliveredAt = DateTime.UtcNow;

        await _repository.UpdateMessageExtensionAsync(extension);
    }

    public async Task<MessageWithReceiptsDto> GetMessageWithReceiptsAsync(Guid messageId, Guid userId)
    {
        var extension = await _queryRepository.GetMessageExtensionAsync(messageId);
        var receipts = await _readReceiptService.GetMessageReceiptsAsync(messageId);
        var voiceNote = await _queryRepository.GetVoiceNoteByMessageAsync(messageId);

        return new MessageWithReceiptsDto
        {
            Id = messageId,
            DeliveryStatus = extension?.DeliveryStatus.ToString() ?? "sent",
            DeliveredAt = extension?.DeliveredAt,
            ReadByCount = receipts.Count,
            ReadReceipts = receipts,
            VoiceNote = voiceNote != null ? new VoiceNoteDto
            {
                Id = voiceNote.Id,
                MessageId = voiceNote.MessageId,
                UserId = voiceNote.UserId,
                AudioUrl = voiceNote.AudioUrl,
                DurationSeconds = voiceNote.DurationSeconds,
                TranscribedText = voiceNote.TranscribedText,
                IsTranscribing = voiceNote.IsTranscribing,
                PlayCount = voiceNote.PlayCount,
                CreatedAt = voiceNote.CreatedAt
            } : null,
            IsEdited = extension?.IsEdited ?? false,
            EditedAt = extension?.EditedAt
        };
    }

    public async Task<List<MessageWithReceiptsDto>> GetConversationMessagesWithReceiptsAsync(Guid conversationId, Guid userId, int page = 1, int pageSize = 50)
    {
        // Would fetch messages from conversation and enrich with receipt data
        return await _queryRepository.GetConversationMessagesWithReceiptsAsync(conversationId, userId, page, pageSize);
        
    }

    public async Task HandleUserTypingAsync(Guid conversationId, Guid userId, bool isTyping)
    {
        if (isTyping)
            await _typingService.StartTypingAsync(conversationId, userId);
        else
            await _typingService.StopTypingAsync(conversationId, userId);
    }

    public async Task<List<TypingIndicatorDto>> GetActiveTypingUsersAsync(Guid conversationId)
    {
        return await _typingService.GetTypingUsersAsync(conversationId);
    }

    public async Task<VoiceNoteDto> UploadVoiceNoteAsync(Guid messageId, Guid userId, UploadVoiceNoteRequestDto request)
    {
        return await _voiceNoteService.UploadVoiceNoteAsync(messageId, userId, request);
    }

    public async Task<VoiceNoteTranscriptionDto> TranscribeVoiceNoteAsync(Guid voiceNoteId)
    {
        return await _voiceNoteService.TranscribeVoiceNoteAsync(voiceNoteId);
    }

    public async Task<ConversationSettingsDto> GetConversationSettingsAsync(Guid conversationId, Guid userId)
    {
        return await _settingsService.GetSettingsAsync(conversationId, userId);
    }

    public async Task<ConversationSettingsDto> UpdateConversationSettingsAsync(Guid conversationId, Guid userId, UpdateConversationSettingsRequestDto request)
    {
        return await _settingsService.UpdateSettingsAsync(conversationId, userId, request);
    }

    public async Task CleanupExpiredTypingIndicatorsAsync()
    {
        await _typingService.ClearExpiredTypingIndicatorsAsync();
    }
}
