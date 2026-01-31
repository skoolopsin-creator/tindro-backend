using Tindro.Application.Common.Interfaces;
using FirebaseAdmin.Messaging;
using System.Text.Json;

namespace Tindro.Infrastructure.Notifications;

public class PushService : IPushService
{
    private readonly IRedisService _redis;
    private readonly INotificationRepository _notificationRepo;

    public PushService(IRedisService redis, INotificationRepository notificationRepo)
    {
        _redis = redis;
        _notificationRepo = notificationRepo;
    }

    public async Task SendAsync(Guid userId, string type, string data)
    {
        var notification = new
        {
            Type = type,
            Data = data,
            CreatedAt = DateTime.UtcNow
        };

        var key = $"notifications:{userId}";
        // Store notifications in Redis with expiry (7 days)
        await _redis.SetAsync(key, JsonSerializer.Serialize(notification), TimeSpan.FromDays(7));

        // Send via FCM (type as title, data as message)
        await SendFcmNotificationAsync(userId, type, data);
    }

    public async Task SendNotificationAsync(Guid userId, string title, string message)
    {
        var notification = new
        {
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        var key = $"notifications:{userId}";
        // Store notifications in Redis with expiry (7 days)
        await _redis.SetAsync(key, JsonSerializer.Serialize(notification), TimeSpan.FromDays(7));

        // Send via FCM
        await SendFcmNotificationAsync(userId, title, message);
    }

    private async Task SendFcmNotificationAsync(Guid userId, string title, string message)
    {
        try
        {
            // Get device tokens for user
            var deviceTokens = await _notificationRepo.GetUserDeviceTokensAsync(userId);
            
            if (deviceTokens == null || !deviceTokens.Any())
                return;

            var messaging = FirebaseMessaging.DefaultInstance;
            
            foreach (var token in deviceTokens)
            {
                var msg = new Message()
                {
                    Token = token,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = message
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "click_action", "FLUTTER_NOTIFICATION_CLICK" },
                        { "timestamp", DateTime.UtcNow.ToString("O") }
                    }
                };

                await messaging.SendAsync(msg);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw - FCM is non-critical
            Console.WriteLine($"FCM send failed: {ex.Message}");
        }
    }
}
