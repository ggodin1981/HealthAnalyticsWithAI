using Microsoft.Extensions.DependencyInjection;

namespace HealthAnalytics.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application-layer components, validators, CQRS, etc.
        return services;
    }
}
