using MediatR;
using Tindro.Application.Chat;


public record GetUserConversations(Guid UserId) : IRequest<List<ConversationDto>>;
