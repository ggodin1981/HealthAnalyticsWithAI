using HealthAnalytics.Application.Patients;
using HealthAnalytics.Application.Patients.Models;
using HealthAnalytics.Domain.Entities;
using HealthAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthAnalytics.Infrastructure.Repositories;

public class PatientService : IPatientService
{
    private readonly HealthAnalyticsDbContext _db;

    public PatientService(HealthAnalyticsDbContext db)
    {
        _db = db;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<IReadOnlyList<PatientListItemDto>> SearchAsync(string? query, int page, int pageSize)
    {
        var q = _db.Patients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            q = q.Where(p =>
                p.Mrn.Contains(query) ||
                p.FirstName.Contains(query) ||
                p.LastName.Contains(query));
        }

        return await q
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatientListItemDto(
                p.Id,
                p.Mrn,
                p.FirstName + " " + p.LastName,
                p.DateOfBirth,
                p.Gender))
            .ToListAsync();
    }

    public async Task<PatientDto> CreateAsync(CreatePatientRequest request)
    {
        var entity = new Patient
        {
            Id = Guid.NewGuid(),
            Mrn = request.Mrn,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender
        };

        _db.Patients.Add(entity);
        await _db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Patient {id} not found");

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.IsActive = request.IsActive;

        await _db.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null) return;

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync();
    }

    private static PatientDto MapToDto(Patient p) =>
        new(p.Id, p.Mrn, p.FirstName, p.LastName, p.DateOfBirth, p.Gender, p.IsActive);
}
