namespace Tindro.Domain.Users;

public class Profile
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Name { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    // compatibility alias used by application code
    public DateTime BirthDate
    {
        get => DateOfBirth;
        set => DateOfBirth = value;
    }
    public string Gender { get; set; } = null!;
    public string Bio { get; set; } = "";

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();

    public System.Collections.Generic.List<string>? Interests { get; set; }


    public int MinAgePreference { get; set; } = 18;
    public int MaxAgePreference { get; set; } = 60;
    public string? GenderPreference { get; set; } // Male, Female, Both
    public string? Education { get; set; }
    public string? IncomeRange { get; set; }

}
