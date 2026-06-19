using Tindro.Domain.Common;
using Tindro.Domain.Users;
namespace Tindro.Domain.Feed;


public class Post : AuditableEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public User User { get; set; } = null!;

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    // Comma separated or JSON string
    public string? Tags { get; private set; }

    public string? MediaUrl { get; private set; }

    public int LikeCount { get; private set; }

    public int CommentCount { get; private set; }

    public int ShareCount { get; private set; }

    private Post() { }

    public Post(
        Guid userId,
        string title,
        string description,
        string? mediaUrl,
        string? tags)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Tags = tags;
        MediaUrl = mediaUrl;

        CreatedAt = DateTime.UtcNow;
    }

    public void Like() => LikeCount++;

    public void Unlike()
    {
        if (LikeCount > 0)
            LikeCount--;
    }

    public void AddComment() => CommentCount++;

    public void RemoveComment()
    {
        if (CommentCount > 0)
            CommentCount--;
    }

    public void AddShare() => ShareCount++;

    public void Update(
        string title,
        string description,
        string? mediaUrl,
        string? tags)
    {
        Title = title;
        Description = description;
        Tags = tags;
        MediaUrl = mediaUrl;

        UpdatedAt = DateTime.UtcNow;
    }
}
