using Carelytix.HealthAnalytics.Application.Ai;
using Carelytix.HealthAnalytics.Application.Auth;
using Carelytix.HealthAnalytics.Application.Patients;
using Carelytix.HealthAnalytics.Infrastructure.Ai;
using Carelytix.HealthAnalytics.Infrastructure.Auth;
using Carelytix.HealthAnalytics.Infrastructure.Data;
using Carelytix.HealthAnalytics.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carelytix.HealthAnalytics.Infrastructure;

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
