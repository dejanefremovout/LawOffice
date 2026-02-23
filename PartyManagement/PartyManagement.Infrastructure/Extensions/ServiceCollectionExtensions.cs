using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Infrastructure.Data;
using PartyManagement.Infrastructure.Repositories;

namespace PartyManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCosmosRepositories(this IServiceCollection services,
        IConfiguration configuration)
    {
        var cosmosConnectionString = configuration["CosmosSettings:ConnectionString"];
        ArgumentException.ThrowIfNullOrWhiteSpace(cosmosConnectionString);
        services.AddSingleton(s => new CosmosClient(cosmosConnectionString));

        services.AddScoped<ICosmosService>(s => new CosmosService(s.GetRequiredService<CosmosClient>()));

        // Register repositories
        //services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IOpposingPartyRepository, OpposingPartyRepository>();

        return services;
    }
}
