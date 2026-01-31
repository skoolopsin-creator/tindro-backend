using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Notifications.Dtos;
using Tindro.Infrastructure.Persistence;

namespace Tindro.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly QueryDbContext _db;
        private readonly Tindro.Application.Common.Interfaces.IRedisService _redis;

        public NotificationRepository(QueryDbContext db, Tindro.Application.Common.Interfaces.IRedisService redis)
        {
            _db = db;
            _redis = redis;
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, string tab, CancellationToken ct)
        {
           

            var list = new List<NotificationDto>();

            if (tab == "likes" || tab == "all")
            {
                var likes = await _db.Swipes
                    .Where(s => s.ToUserId == userId && s.IsLike)
                    .OrderByDescending(s => s.CreatedAt)
                    .Select(s => new NotificationDto
                    {
                        Id = s.Id,
                        Type = "like",
                        ActorId = s.FromUserId.ToString(),
                        CreatedAt = s.CreatedAt,
                        Message = null,
                        IsRead = false
                    })
                    .ToListAsync(ct).ConfigureAwait(false);

                list.AddRange(likes);
            }

            if (tab == "matches" || tab == "all")
            {
                var matches = await _db.Matches
                    .Where(m => m.User1Id == userId || m.User2Id == userId)
                    .OrderByDescending(m => m.MatchedAt)
                    .Select(m => new NotificationDto
                    {
                        Id = m.Id,
                        Type = "match",
                        ActorId = (m.User1Id == userId ? m.User2Id : m.User1Id).ToString(),
                        CreatedAt = m.MatchedAt,
                        Message = null,
                        IsRead = false
                    })
                    .ToListAsync(ct).ConfigureAwait(false);

                list.AddRange(matches);
            }

            // Enrich with IsRead from Redis set 'notifications:read:{userId}'
            var readKey = $"notifications:read:{userId}";
            foreach (var n in list)
            {
                n.IsRead = await _redis.SetContainsAsync(readKey, n.Id.ToString()).ConfigureAwait(false);
            }

            return list.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public async Task SaveDeviceTokenAsync(Guid userId, string token, string platform, CancellationToken ct)
        {
            // This should save device token to database for FCM push notifications
            // Implementation depends on DeviceToken entity schema
            // For now, store in Redis cache as backup
            var key = $"device_tokens:{userId}:{platform}";
            await _redis.SetAsync(key, token, TimeSpan.FromDays(30)).ConfigureAwait(false);
        }

        public async Task<List<string>> GetUserDeviceTokensAsync(Guid userId)
        {
            // Retrieve all device tokens for a user
            // This should query the database for stored tokens
            // For now, return tokens from Redis if available
            var tokens = new List<string>();
            
            // Get tokens from different platforms
            var androidKey = $"device_tokens:{userId}:android";
            var iosKey = $"device_tokens:{userId}:ios";
            
            var androidToken = await _redis.GetAsync(androidKey).ConfigureAwait(false);
            var iosToken = await _redis.GetAsync(iosKey).ConfigureAwait(false);
            
            if (!string.IsNullOrEmpty(androidToken))
                tokens.Add(androidToken);
            if (!string.IsNullOrEmpty(iosToken))
                tokens.Add(iosToken);
            
            return tokens;
        }
    }
}
