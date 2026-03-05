using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Application.Services;
using OfficeManagement.Infrastructure.Extensions;

namespace OfficeManagement.Application.Extensions;

/// <summary>
/// Dependency injection registration for application layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services and required infrastructure repositories.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register infrastructure repository dependencies.
        services.AddCosmosRepositories(configuration);

        // Register application services.
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<ILawyerService, LawyerService>();

        return services;
    }
}
