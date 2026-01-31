using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Chat.Queries
{
    public class GetUserConversationsQueryHandler
    : IRequestHandler<GetUserConversations, List<ConversationDto>>
    {
        private readonly IChatRepository _chatRepo;

        public GetUserConversationsQueryHandler(IChatRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        public async Task<List<ConversationDto>> Handle(GetUserConversations q, CancellationToken ct)
        {
            var conversations = await _chatRepo.GetUserConversationsAsync(q.UserId, ct);
            return conversations;
        }
    }

}
