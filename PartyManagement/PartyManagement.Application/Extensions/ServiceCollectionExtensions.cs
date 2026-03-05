using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartyManagement.Application.Services;
using PartyManagement.Infrastructure.Extensions;

namespace PartyManagement.Application.Extensions;

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
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IOpposingPartyService, OpposingPartyService>();

        return services;
    }
}
