using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Infrastructure.Data;
using OfficeManagement.Infrastructure.Repositories;

namespace OfficeManagement.Infrastructure.Extensions;

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
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<ILawyerRepository, LawyerRepository>();

        return services;
    }
}
