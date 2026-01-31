using StackExchange.Redis;
using System.Text.Json;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Infrastructure.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _mux;

        public RedisService(IConnectionMultiplexer mux)
        {
            _mux = mux;
        }

        public async Task SetAsync(string key, string value)
        {
            var db = _mux.GetDatabase();
            await db.StringSetAsync(key, value).ConfigureAwait(false);
        }

        public async Task SetAsync(string key, string value, TimeSpan ttl)
        {
            var db = _mux.GetDatabase();
            await db.StringSetAsync(key, value, ttl).ConfigureAwait(false);
        }

        public async Task SetAsync(string key, double value, TimeSpan ttl, CancellationToken ct)
        {
            var db = _mux.GetDatabase();
            await db.StringSetAsync(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture)).ConfigureAwait(false);
            await db.KeyExpireAsync(key, ttl).ConfigureAwait(false);
        }

        public async Task SetAsync(string key, string[] values, TimeSpan ttl, CancellationToken ct)
        {
            var db = _mux.GetDatabase();
            // store as JSON array
            var json = JsonSerializer.Serialize(values);
            await db.StringSetAsync(key, json).ConfigureAwait(false);
            await db.KeyExpireAsync(key, ttl).ConfigureAwait(false);
        }

        public async Task<string?> GetAsync(string key)
        {
            var db = _mux.GetDatabase();
            var val = await db.StringGetAsync(key).ConfigureAwait(false);
            return val.HasValue ? val.ToString() : null;
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken ct)
        {
            // pattern is a Redis key pattern (e.g. "stories:*")
            var endpoints = _mux.GetEndPoints();
            foreach (var ep in endpoints)
            {
                var server = _mux.GetServer(ep);
                // Keys can be enumerable; delete matching keys
                foreach (var key in server.Keys(database: _mux.GetDatabase().Database, pattern: pattern))
                {
                    await _mux.GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);
                }
            }
        }

        public async Task SetAddAsync(string key, string value)
        {
            var db = _mux.GetDatabase();
            await db.SetAddAsync(key, value).ConfigureAwait(false);
        }

        public async Task<bool> SetContainsAsync(string key, string value)
        {
            var db = _mux.GetDatabase();
            return await db.SetContainsAsync(key, value).ConfigureAwait(false);
        }

        public async Task SetRemoveAsync(string key, string value)
        {
            var db = _mux.GetDatabase();
            await db.SetRemoveAsync(key, value).ConfigureAwait(false);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var db = _mux.GetDatabase();
            return await db.KeyExistsAsync(key).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string key)
        {
            var db = _mux.GetDatabase();
            await db.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public async Task<bool> AllowAsync(string key, int limit, TimeSpan window)
        {
            var db = _mux.GetDatabase();
            var current = await db.StringGetAsync(key).ConfigureAwait(false);
            
            if (!current.HasValue)
            {
                await db.StringSetAsync(key, "1", window).ConfigureAwait(false);
                return true;
            }

            if (int.TryParse(current.ToString(), out var count) && count < limit)
            {
                await db.StringIncrementAsync(key).ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}

