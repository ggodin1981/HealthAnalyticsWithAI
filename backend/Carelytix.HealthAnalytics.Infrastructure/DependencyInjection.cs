using HealthAnalytics.Application.Ai;
using HealthAnalytics.Application.Auth;
using HealthAnalytics.Application.Patients;
using HealthAnalytics.Infrastructure.Ai;
using HealthAnalytics.Infrastructure.Auth;
using HealthAnalytics.Infrastructure.Data;
using HealthAnalytics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HealthAnalytics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Postgres")
                               ?? "Host=localhost;Port=5432;Database=carelytix;Username=carelytix;Password=changeme";

        services.AddDbContext<HealthAnalyticsDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAnalyticsAssistantService, LlmAnalyticsAssistantService>();

        services.AddHttpClient("ai");

        return services;
    }
}
