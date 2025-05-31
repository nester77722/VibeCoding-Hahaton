using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Currently empty as we don't have any domain services to register
        return services;
    }
} 