using Azure.Storage.Blobs;
using CaseManagement.Application.Services;
using CaseManagement.Domain.Interfaces;
using CaseManagement.Infrastructure.Data;
using CaseManagement.Infrastructure.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CaseManagement.Api.Extensions;

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
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IHearingService, HearingService>();
        services.AddScoped<IDocumentFileService, DocumentFileService>();

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

        var blobConnectionString = configuration["BlobSettings:ConnectionString"];
        ArgumentException.ThrowIfNullOrWhiteSpace(blobConnectionString);
        services.AddSingleton(s => new BlobServiceClient(blobConnectionString));
        services.AddScoped<IBlobService>(s => new BlobService(s.GetRequiredService<BlobServiceClient>()));

        // Register repositories
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<IHearingRepository, HearingRepository>();
        services.AddScoped<IDocumentFileRepository, DocumentFileRepository>();
        services.AddScoped<IDocumentFileStorageRepository, DocumentFileStorageRepository>();

        return services;
    }
}
