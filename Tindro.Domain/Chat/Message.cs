namespace Tindro.Domain.Chat;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }

    public string Text { get; set; } = null!;
    public string? CipherText { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
