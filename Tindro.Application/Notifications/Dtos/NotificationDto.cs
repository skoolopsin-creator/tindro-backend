using System;

namespace Tindro.Application.Notifications.Dtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // e.g. "like", "match"
        public string ActorId { get; set; } = string.Empty; // who performed the action
        public DateTime CreatedAt { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
    }
}
