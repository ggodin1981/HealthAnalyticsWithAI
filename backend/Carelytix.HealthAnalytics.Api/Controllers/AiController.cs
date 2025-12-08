using HealthAnalytics.Application.Ai;
using HealthAnalytics.Application.Ai.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Clinician,Analyst")]
public class AiController : ControllerBase
{
    private readonly IAnalyticsAssistantService _assistantService;

    public AiController(IAnalyticsAssistantService assistantService)
    {
        _assistantService = assistantService;
    }

    /// <summary>
    /// Ask the AI analytics assistant a question about population / patient cohorts.
    /// Optionally bind to a specific patient MRN.
    /// </summary>
    [HttpPost("analyze")]
    public async Task<ActionResult<AnalyzeResponse>> Analyze(AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var result = await _assistantService.AnalyzeAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Chat-style endpoint for an analytics / architecture co-pilot.
    /// </summary>
    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat(ChatRequest request, CancellationToken cancellationToken)
    {
        var result = await _assistantService.ChatAsync(request, cancellationToken);
        return Ok(result);
    }
}
