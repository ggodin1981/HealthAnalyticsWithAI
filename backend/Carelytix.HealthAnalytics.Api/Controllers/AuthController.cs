using Carelytix.HealthAnalytics.Application.Auth;
using Carelytix.HealthAnalytics.Application.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Carelytix.HealthAnalytics.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login with username/password to obtain a JWT bearer token.
    /// Demo user: admin / Admin123!
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.UserName, request.Password);
        if (result is null)
            return Unauthorized(new { message = "Invalid credentials" });

        var response = new LoginResponse(result.AccessToken, result.ExpiresAt, result.UserName, result.Role);
        return Ok(response);
    }
}
