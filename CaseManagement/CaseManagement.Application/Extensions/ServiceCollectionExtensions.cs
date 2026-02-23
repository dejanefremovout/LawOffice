using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CaseManagement.Application.Services;
using CaseManagement.Infrastructure.Extensions;

namespace CaseManagement.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Register Cosmos DB related services
        services.AddCosmosRepositories(configuration);

        // Register services
        services.AddScoped<ICaseService, CaseService>();
        services.AddScoped<IHearingService, HearingService>();
        services.AddScoped<IDocumentFileService, DocumentFileService>();

        return services;
    }
}
