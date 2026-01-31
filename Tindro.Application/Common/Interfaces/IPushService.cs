namespace Tindro.Application.Common.Interfaces;

public interface IPushService
{
    Task SendAsync(Guid userId, string type, string data);
    Task SendNotificationAsync(Guid userId, string title, string message);
}
