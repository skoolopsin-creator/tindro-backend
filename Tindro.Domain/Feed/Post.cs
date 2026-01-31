using Tindro.Domain.Common;

namespace Tindro.Domain.Feed;

public class Post : AuditableEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; } 
    public string Content { get; private set; } = null!;
    public string? MediaUrl { get; private set; }
    public int LikeCount { get; private set; }
    public int CommentCount { get; private set; }

    private Post() { }

    public Post(Guid userId, string content, string? mediaUrl)
    {
        UserId = userId;
        Content = content;
        MediaUrl = mediaUrl;
        CreatedAt = DateTime.UtcNow;
    }

    public void Like() => LikeCount++;
    public void Unlike() => LikeCount--;
    public void AddComment() => CommentCount++;
    public void RemoveComment() => CommentCount--;
}
