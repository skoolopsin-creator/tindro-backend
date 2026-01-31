using Microsoft.EntityFrameworkCore;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Chat;
using Tindro.Domain.Chat;

namespace Tindro.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly QueryDbContext _db;

        public ChatRepository(QueryDbContext db)
        {
            _db = db;
        }

        public async Task AddMessageAsync(Tindro.Domain.Chat.Message message, CancellationToken ct)
        {
            _db.Messages.Add(message);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<MessageDto>> GetConversationAsync(Guid matchId, CancellationToken ct)
        {
            var msgs = await _db.Messages
                .Where(m => m.ConversationId == matchId)
                .OrderBy(m => m.SentAt)
                .ToListAsync(ct);

            return msgs.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                Text = m.Text,
                SentAt = m.SentAt,
                IsRead = m.IsRead
            }).ToList();
        }

        public async Task<List<ConversationDto>> GetUserConversationsAsync(Guid userId, CancellationToken ct)
        {
          

            var convos = await _db.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .ToListAsync(ct);

            var result = new List<ConversationDto>();
            foreach (var c in convos)
            {
                var last = await _db.Messages.Where(m => m.ConversationId == c.Id)
                    .OrderByDescending(m => m.SentAt).FirstOrDefaultAsync(ct);

                result.Add(new ConversationDto
                {
                    MatchId = c.Id,
                    LastMessage = last?.Text,
                    LastMessageAt = last?.SentAt
                });
            }

            return result;
        }

        public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct)
        {
            
            return _db.Messages.CountAsync(m => m.SenderId != userId && !m.IsRead, ct);
        }

        public async Task MarkAsReadAsync(Guid matchId, Guid userId, CancellationToken ct)
        {
          
            var msgs = await _db.Messages.Where(m => m.ConversationId == matchId && m.SenderId != userId && !m.IsRead).ToListAsync(ct);
            foreach (var m in msgs) m.IsRead = true;
            await _db.SaveChangesAsync(ct);
        }
    }
}

