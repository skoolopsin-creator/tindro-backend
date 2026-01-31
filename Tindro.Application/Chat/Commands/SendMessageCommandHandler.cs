using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Chat;

namespace Tindro.Application.Chat.Commands
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessage>
    {
        private readonly IChatRepository _repo;

        public SendMessageCommandHandler(IChatRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(SendMessage cmd, CancellationToken ct)
        {
            var msg = new Tindro.Domain.Chat.Message
            {
                Id = Guid.NewGuid(),
                ConversationId = cmd.ConversationId,
                SenderId = cmd.SenderId,
                Text = cmd.Text,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _repo.AddMessageAsync(msg, ct);
        }
    }

}
