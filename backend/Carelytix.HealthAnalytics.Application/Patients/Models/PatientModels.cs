namespace Carelytix.HealthAnalytics.Application.Patients.Models;

public record PatientDto(
    Guid Id,
    string Mrn,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender,
    bool IsActive
);

public record PatientListItemDto(
    Guid Id,
    string Mrn,
    string FullName,
    DateTime DateOfBirth,
    string Gender
);

public record CreatePatientRequest(
    string Mrn,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Gender
);

public record UpdatePatientRequest(
    string FirstName,
    string LastName,
    bool IsActive
);
