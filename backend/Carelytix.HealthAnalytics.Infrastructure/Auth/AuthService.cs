using System.Security.Cryptography;
using System.Text;
using Carelytix.HealthAnalytics.Application.Auth;
using Carelytix.HealthAnalytics.Application.Auth.Models;
using Carelytix.HealthAnalytics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Carelytix.HealthAnalytics.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly HealthAnalyticsDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthService(HealthAnalyticsDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<AuthResult?> LoginAsync(string userName, string password)
    {
        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);

        if (user is null)
            return null;

        var incomingHash = ComputeSha256(password);
        if (!string.Equals(incomingHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
            return null;

        var jwtSection = _configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"] ?? "carelytix-local";
        var audience = jwtSection["Audience"] ?? "carelytix-clients";
        var key = jwtSection["Key"] ?? "THIS_IS_NOT_SECURE_CHANGE_ME_FOR_PROD";

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        };

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResult(tokenString, expires, user.UserName, user.Role);
    }

    private static string ComputeSha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
