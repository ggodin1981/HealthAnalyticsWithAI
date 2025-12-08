using Microsoft.Extensions.DependencyInjection;

namespace Carelytix.HealthAnalytics.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application-layer components, validators, CQRS, etc.
        return services;
    }
}
