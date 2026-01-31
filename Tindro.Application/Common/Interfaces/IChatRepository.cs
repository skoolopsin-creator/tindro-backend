using Tindro.Domain.Chat;

namespace Tindro.Application.Common.Interfaces;

public interface IChatRepository
{
    Task<List<Tindro.Application.Chat.MessageDto>> GetConversationAsync(Guid matchId, CancellationToken ct);
    Task<List<Tindro.Application.Chat.ConversationDto>> GetUserConversationsAsync(Guid userId, CancellationToken ct);
    Task AddMessageAsync(Tindro.Domain.Chat.Message message, CancellationToken ct);
    Task MarkAsReadAsync(Guid matchId, Guid userId, CancellationToken ct);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
}
