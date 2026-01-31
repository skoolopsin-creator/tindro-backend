using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tindro.Application.Chat;
using Tindro.Api.Extensions;
using Tindro.Application.Chat.Commands;

namespace Tindro.Api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = User.GetUserId();
            return Ok(await _mediator.Send(new GetUserConversations(userId)));
        }

        [HttpGet("{matchId}")]
        public async Task<IActionResult> GetChat(Guid matchId)
        {
            return Ok(await _mediator.Send(new GetConversation(matchId)));
        }

        [HttpPost("{matchId}/send")]
        public async Task<IActionResult> Send(Guid matchId, [FromBody] string text)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new SendMessage(matchId, userId, text));
            return Ok();
        }

        [HttpPost("{matchId}/read")]
        public async Task<IActionResult> Read(Guid matchId)
        {
            var userId = User.GetUserId();
            await _mediator.Send(new MarkMessagesAsReadCommand(matchId, userId));
            return Ok();
        }
    }

}
