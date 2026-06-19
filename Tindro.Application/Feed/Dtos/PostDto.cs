namespace Tindro.Application.Feed.Dtos;

public class PostDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public List<string> Tags { get; set; } = new();

    public string? MediaUrl { get; set; }

    public int LikeCount { get; set; }

    public int CommentCount { get; set; }

    public int ShareCount { get; set; }

    public DateTime CreatedAt { get; set; }


}
