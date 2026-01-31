using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Tindro.Application.Chat
{


    public record GetConversation(Guid MatchId) : IRequest<List<MessageDto>>;


}
