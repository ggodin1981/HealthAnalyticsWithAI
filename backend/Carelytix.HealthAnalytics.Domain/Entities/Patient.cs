namespace HealthAnalytics.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; }
    public string Mrn { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
}
