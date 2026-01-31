using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Chat.Commands
{
    public class MarkMessagesAsReadCommandHandler
     : IRequestHandler<MarkMessagesAsReadCommand>
    {
        private readonly IChatRepository _repo;

        public MarkMessagesAsReadCommandHandler(IChatRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(MarkMessagesAsReadCommand cmd, CancellationToken ct)
        {
            await _repo.MarkAsReadAsync(cmd.MatchId, cmd.UserId, ct);
        }
    }

}
