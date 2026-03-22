using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartyManagement.Api.Messaging;
using PartyManagement.Application.Services;
using PartyManagement.Domain.Interfaces;
using PartyManagement.Infrastructure.Data;
using PartyManagement.Infrastructure.Repositories;

namespace PartyManagement.Api.Extensions;

/// <summary>
/// Dependency injection registration
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services and required infrastructure repositories.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services.
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IOpposingPartyService, OpposingPartyService>();
        services.AddScoped<IOpposingPartyDeletedPublisher, ServiceBusOpposingPartyDeletedPublisher>();

        return services;
    }

    /// <summary>
    /// Registers infrastructure repository dependencies.
    /// </summary>
    public static IServiceCollection AddCosmosRepositories(this IServiceCollection services,
        IConfiguration configuration)
    {
        var cosmosConnectionString = configuration["CosmosSettings:ConnectionString"];
        ArgumentException.ThrowIfNullOrWhiteSpace(cosmosConnectionString);
        services.AddSingleton(s => new CosmosClient(cosmosConnectionString));

        services.AddScoped<ICosmosService>(s => new CosmosService(s.GetRequiredService<CosmosClient>()));

        // Register repositories
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IOpposingPartyRepository, OpposingPartyRepository>();

        return services;
    }

    /// <summary>
    /// Registers Azure Service Bus dependencies for messaging.
    /// </summary>
    public static IServiceCollection AddServiceBusDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        var serviceBusConnectionString = configuration["ServiceBusConnectionString"];
        ArgumentException.ThrowIfNullOrWhiteSpace(serviceBusConnectionString);

        var opposingPartyDeletedQueueName = configuration["OpposingPartyDeletedQueueName"];
        ArgumentException.ThrowIfNullOrWhiteSpace(opposingPartyDeletedQueueName);

        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));
        services.AddScoped(s => s.GetRequiredService<ServiceBusClient>().CreateSender(opposingPartyDeletedQueueName));

        return services;
    }
}
