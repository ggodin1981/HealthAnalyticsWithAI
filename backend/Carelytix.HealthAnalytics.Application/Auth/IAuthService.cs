using HealthAnalytics.Application.Auth.Models;

namespace HealthAnalytics.Application.Auth;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(string userName, string password);
}
