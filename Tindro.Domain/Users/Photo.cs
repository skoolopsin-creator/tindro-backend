namespace Tindro.Domain.Users;

public class Photo
{
    public Guid Id { get; set; }
    public Guid ProfileId { get; set; }

    public string Url { get; set; } = null!;
    public bool IsMain { get; set; }
}
