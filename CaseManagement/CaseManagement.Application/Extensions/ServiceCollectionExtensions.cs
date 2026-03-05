using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CaseManagement.Application.Services;
using CaseManagement.Infrastructure.Extensions;

namespace CaseManagement.Application.Extensions;

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
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IHearingService, HearingService>();
        services.AddScoped<IDocumentFileService, DocumentFileService>();

        return services;
    }
}
