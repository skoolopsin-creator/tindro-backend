using Microsoft.EntityFrameworkCore;
using Tindro.Application.Chat.Dtos;
using Tindro.Application.Chat.Interfaces;
using Tindro.Domain.Chat;
using Tindro.Infrastructure.Persistence;

public class MessagingQueryRepository : IMessagingQueryRepository
{
    private readonly QueryDbContext _context;

    public MessagingQueryRepository(QueryDbContext context)
    {
        _context = context;
    }

    public async Task<MessageReadReceipt?> GetReceiptAsync(Guid messageId, Guid userId)
    {
        return await _context.MessageReadReceipts
            .FirstOrDefaultAsync(r => r.MessageId == messageId && r.UserId == userId);
    }

    public async Task<List<MessageReadReceipt>> GetMessageReceiptsAsync(Guid messageId)
    {
        return await _context.MessageReadReceipts
            .Where(r => r.MessageId == messageId)
            .OrderByDescending(r => r.ReadAt)
            .ToListAsync();
    }

    public async Task<int> GetReadCountAsync(Guid messageId)
    {
        return await _context.MessageReadReceipts
            .CountAsync(r => r.MessageId == messageId);
    }

    public async Task<TypingIndicator?> GetActiveTypingIndicatorAsync(Guid conversationId, Guid userId)
    {
        return await _context.TypingIndicators
            .Where(t => t.ConversationId == conversationId && t.UserId == userId && t.IsActive)
            .OrderByDescending(t => t.StartedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TypingIndicator>> GetConversationTypingIndicatorsAsync(Guid conversationId)
    {
        return await _context.TypingIndicators
            .Where(t => t.ConversationId == conversationId)
            .OrderByDescending(t => t.StartedAt)
            .ToListAsync();
    }

    public async Task<VoiceNote?> GetVoiceNoteAsync(Guid voiceNoteId)
    {
        return await _context.VoiceNotes.FirstOrDefaultAsync(v => v.Id == voiceNoteId);
    }

    public async Task<VoiceNote?> GetVoiceNoteByMessageAsync(Guid messageId)
    {
        return await _context.VoiceNotes.FirstOrDefaultAsync(v => v.MessageId == messageId);
    }


    public async Task<List<VoiceNote>> GetConversationVoiceNotesAsync(Guid conversationId, int limit = 50)
    {
        // Would need to join with Messages to filter by conversation
        return await _context.VoiceNotes
            .OrderByDescending(v => v.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
    public async Task<MessageExtension?> GetMessageExtensionAsync(Guid messageId)
    {
        return await _context.MessageExtensions
            .Include(m => m.ReadReceipts)
            .FirstOrDefaultAsync(m => m.MessageId == messageId);
    }

    public async Task<ConversationSettings?> GetConversationSettingsAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationSettings
            .FirstOrDefaultAsync(s => s.ConversationId == conversationId && s.UserId == userId);
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId)
    {
        return await _context.Messages
            .Where(m =>
                m.ConversationId == conversationId &&
                m.SenderId != userId &&
                !_context.MessageReadReceipts.Any(r =>
                    r.MessageId == m.Id && r.UserId == userId))
            .CountAsync();
    }

    public async Task<Dictionary<Guid, int>> GetUnreadCountsByConversationAsync(Guid userId)
    {
        var unreadCounts = await _context.Messages
            .Where(m =>
                m.SenderId != userId &&
                !_context.MessageReadReceipts.Any(r =>
                    r.MessageId == m.Id && r.UserId == userId))
            .GroupBy(m => m.ConversationId)
            .Select(g => new
            {
                ConversationId = g.Key,
                Count = g.Count()
            })
            .ToListAsync();

        return unreadCounts.ToDictionary(x => x.ConversationId, x => x.Count);
    }

    public async Task<List<MessageWithReceiptsDto>> GetConversationMessagesWithReceiptsAsync(
    Guid conversationId,
    Guid userId,
    int page = 1,
    int pageSize = 50)
    {
        var skip = (page - 1) * pageSize;

        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip(skip)
            .Take(pageSize)
            .Select(m => new MessageWithReceiptsDto
            {
                Id = m.Id,
                ConversationId = m.ConversationId,
                SenderId = m.SenderId,

                Content = m.Text,
                SentAt = m.SentAt,

            })
            .ToListAsync();
    }


}
