namespace HealthAnalytics.Application.Auth.Models;

public record AuthResult(
    string AccessToken,
    DateTime ExpiresAt,
    string UserName,
    string Role
);

public record LoginRequest(
    string UserName,
    string Password
);

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string UserName,
    string Role
);
