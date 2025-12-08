namespace Carelytix.HealthAnalytics.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
}
