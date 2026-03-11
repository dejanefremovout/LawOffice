using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Infrastructure.Data;
using OfficeManagement.Infrastructure.Repositories;

namespace OfficeManagement.Api.Extensions;

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
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<ILawyerService, LawyerService>();

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
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<ILawyerRepository, LawyerRepository>();

        return services;
    }
}