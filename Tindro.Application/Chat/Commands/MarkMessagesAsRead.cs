using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Chat
{
    public record MarkMessagesAsReadCommand(Guid MatchId, Guid UserId) : IRequest;

}
