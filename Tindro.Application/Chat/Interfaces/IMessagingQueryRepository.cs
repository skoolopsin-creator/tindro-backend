using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Chat.Dtos;
using Tindro.Domain.Chat;

namespace Tindro.Application.Chat.Interfaces
{
    public interface IMessagingQueryRepository
    {

        Task<MessageReadReceipt?> GetReceiptAsync(Guid messageId, Guid userId);

        Task<List<MessageReadReceipt>> GetMessageReceiptsAsync(Guid messageId);
        Task<int> GetReadCountAsync(Guid messageId);

        // Typing indicators
        Task<TypingIndicator?> GetActiveTypingIndicatorAsync(Guid conversationId, Guid userId);


        Task<List<TypingIndicator>> GetConversationTypingIndicatorsAsync(Guid conversationId);

        // Voice notes
        Task<VoiceNote?> GetVoiceNoteAsync(Guid voiceNoteId);
        Task<VoiceNote?> GetVoiceNoteByMessageAsync(Guid messageId);

        Task<List<VoiceNote>> GetConversationVoiceNotesAsync(Guid conversationId, int limit = 50);

        // Message extensions
        Task<MessageExtension?> GetMessageExtensionAsync(Guid messageId);


        // Conversation settings
        Task<ConversationSettings?> GetConversationSettingsAsync(Guid conversationId, Guid userId);

        Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);

        Task<Dictionary<Guid, int>> GetUnreadCountsByConversationAsync(Guid userId);

        Task<List<MessageWithReceiptsDto>> GetConversationMessagesWithReceiptsAsync(
Guid conversationId,
Guid userId,
int page = 1,
int pageSize = 50);
    }
}
