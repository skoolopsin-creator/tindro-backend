using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Chat.Queries
{
    public class GetConversationQueryHandler : IRequestHandler<GetConversation, List<MessageDto>>
    {
        private readonly IChatRepository _repo;

        public GetConversationQueryHandler(IChatRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<MessageDto>> Handle(GetConversation q, CancellationToken ct)
        {
            var messages = await _repo.GetConversationAsync(q.MatchId, ct);
            return messages.OrderBy(m => m.SentAt).ToList();
        }
    }

}
