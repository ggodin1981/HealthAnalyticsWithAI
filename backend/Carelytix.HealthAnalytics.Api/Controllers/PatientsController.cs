using HealthAnalytics.Application.Patients;
using HealthAnalytics.Application.Patients.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HealthAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Clinician")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PatientListItemDto>>> Search(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var results = await _patientService.SearchAsync(q, page, pageSize);
        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientRequest request)
    {
        var created = await _patientService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PatientDto>> Update(Guid id, UpdatePatientRequest request)
    {
        var updated = await _patientService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _patientService.DeleteAsync(id);
        return NoContent();
    }
}
