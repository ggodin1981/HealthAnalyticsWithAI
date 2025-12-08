using Carelytix.HealthAnalytics.Application.Auth.Models;

namespace Carelytix.HealthAnalytics.Application.Auth;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(string userName, string password);
}
