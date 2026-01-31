namespace Tindro.Domain.Payments;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Plan { get; set; } = null!; // Free, Plus, Gold
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive => EndDate > DateTime.UtcNow;
}
