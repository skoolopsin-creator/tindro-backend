using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Common.Interfaces
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value);
        Task SetAsync(string key, string value, TimeSpan ttl);
        Task SetAsync(string key, double value, TimeSpan ttl, CancellationToken ct);
        Task SetAsync(string key, string[] values, TimeSpan ttl, CancellationToken ct);
        Task<string?> GetAsync(string key);
        Task RemoveByPatternAsync(string pattern, CancellationToken ct);
        Task SetAddAsync(string key, string value);
        Task<bool> SetContainsAsync(string key, string value);
        Task SetRemoveAsync(string key, string value);
        Task<bool> ExistsAsync(string key);
        Task DeleteAsync(string key);
        Task<bool> AllowAsync(string key, int limit, TimeSpan window);
    }
}
