using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Tindro.Domain.Chat;
using Tindro.Infrastructure.Persistence;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
     private readonly CommandDbContext _commandDb;
    private readonly QueryDbContext _queryDb;
    private readonly IRedisService _redis;
    private readonly IEncryptionService _crypto;
    private readonly IPushService _push;

    public ChatHub(
        CommandDbContext commandDb,
        QueryDbContext queryDb,
        IRedisService redis,
        IEncryptionService crypto,
        IPushService push)
    {
        _commandDb = commandDb;
        _queryDb = queryDb;
        _redis = redis;
        _crypto = crypto;
        _push = push;
    }

    public async Task JoinConversation(Guid conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendMessage(Guid conversationId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        if (text.Length > 2000)
            return;

        var userId = Guid.Parse(
            Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        // Rate limit (30 msg/min)
        var key = $"chat:rate:{userId}";
        if (!await _redis.AllowAsync(key, 30, TimeSpan.FromMinutes(1)))
            return;

        var encrypted = _crypto.Encrypt(text);

        var msg = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = userId,
            CipherText = encrypted,
            CreatedAt = DateTime.UtcNow
        };

        _commandDb.Messages.Add(msg);
        await _commandDb.SaveChangesAsync();

        // Send to online users
        await Clients.Group(conversationId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                msg.Id,
                msg.SenderId,
                Text = text,
                msg.CreatedAt
            });

        // Check if receiver offline
        var receiverId = GetOtherParticipant(conversationId, userId);
        var online = await _redis.ExistsAsync($"online:{receiverId}");

        if (!online)
        {
            await _push.SendAsync(
                receiverId,
                "NEW_MESSAGE",
                conversationId.ToString()
            );
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User!
            .FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _redis.SetAsync(
            $"online:{userId}",
            "1",
            TimeSpan.FromMinutes(2)
        );

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        var userId = Context.User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != null)
            await _redis.DeleteAsync($"online:{userId}");

        await base.OnDisconnectedAsync(ex);
    }

    private Guid GetOtherParticipant(Guid conversationId, Guid senderId)
    {
        return _queryDb.Conversations
            .Where(x => x.Id == conversationId)
            .SelectMany(x => x.Participants)
            .Where(p => p.UserId != senderId)
            .Select(p => p.UserId)
            .First();
    }
}
