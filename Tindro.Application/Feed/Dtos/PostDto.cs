namespace Tindro.Application.Feed.Dtos;

public class PostDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public string Content { get; set; } = null!;
    public string? MediaUrl { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
