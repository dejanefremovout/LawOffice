using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfficeManagement.Application.Services;
using OfficeManagement.Infrastructure.Extensions;

namespace OfficeManagement.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Register Cosmos DB related services
        services.AddCosmosRepositories(configuration);

        // Register services
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<ILawyerService, LawyerService>();

        return services;
    }
}
