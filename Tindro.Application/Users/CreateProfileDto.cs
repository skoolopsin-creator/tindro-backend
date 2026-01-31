public class CreateProfileDto
{
    public string Name { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Bio { get; set; } = "";


    public int MinAgePreference { get; set; }
    public int MaxAgePreference { get; set; }
    public string? GenderPreference { get; set; }
    public string? Education { get; set; }
    public string? IncomeRange { get; set; }

}
