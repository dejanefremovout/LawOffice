using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartyManagement.Application.Services;
using PartyManagement.Infrastructure.Extensions;

namespace PartyManagement.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Register Cosmos DB related services
        services.AddCosmosRepositories(configuration);

        // Register services
        //services.AddScoped<IPartyService, PartyService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IOpposingPartyService, OpposingPartyService>();

        return services;
    }
}
