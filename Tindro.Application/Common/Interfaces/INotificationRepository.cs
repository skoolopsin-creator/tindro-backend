using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tindro.Application.Notifications.Dtos;

namespace Tindro.Application.Common.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<NotificationDto>> GetNotificationsAsync(Guid userId, string tab, CancellationToken ct);
        Task SaveDeviceTokenAsync(Guid userId, string token, string platform, CancellationToken ct);
        Task<List<string>> GetUserDeviceTokensAsync(Guid userId);
    }
}
