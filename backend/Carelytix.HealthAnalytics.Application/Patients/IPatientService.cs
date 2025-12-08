using HealthAnalytics.Application.Patients.Models;

namespace HealthAnalytics.Application.Patients;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<PatientListItemDto>> SearchAsync(string? query, int page, int pageSize);
    Task<PatientDto> CreateAsync(CreatePatientRequest request);
    Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request);
    Task DeleteAsync(Guid id);
}
