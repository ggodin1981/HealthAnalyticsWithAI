namespace HealthAnalytics.Domain.Entities;

public class Encounter
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public DateTime EncounterDate { get; set; }
    public string EncounterType { get; set; } = null!;
    public string DiagnosisCode { get; set; } = null!;
    public decimal RiskScore { get; set; }

    public Patient Patient { get; set; } = null!;
}
