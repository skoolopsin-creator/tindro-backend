using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Chat.Commands
{
    public record SendMessage(Guid ConversationId, Guid SenderId, string Text)
    : IRequest;
}
