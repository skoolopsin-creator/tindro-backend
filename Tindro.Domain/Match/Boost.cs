namespace Tindro.Domain.Match;

public class Boost
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ActiveUntil { get; set; }
    public bool IsActive => ActiveUntil > DateTime.UtcNow;
}
