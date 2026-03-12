using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using OfficeManagement.Domain.Interfaces;

namespace OfficeManagement.Infrastructure;

/// <summary>
/// Reads user profile data from Microsoft Entra via the Graph SDK.
/// </summary>
public class GraphUserService : IGraphUserService
{
    private readonly GraphServiceClient _graphClient;
    private readonly string _officeNameKey;

    public GraphUserService(GraphServiceClient graphClient, IConfiguration configuration)
    {
        _graphClient = graphClient;

        var appId = (configuration["EntraAppId"] ?? string.Empty)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        _officeNameKey = $"extension_{appId}_OfficeName";
    }

    /// <inheritdoc />
    public async Task<string?> GetOfficeName(string email)
    {
        var result = await _graphClient.Users
            .GetAsync(config =>
            {
                config.QueryParameters.Filter = $"mail eq '{email}'";
                config.QueryParameters.Select = ["id", _officeNameKey];
                config.QueryParameters.Top = 1;
            });

        var user = result?.Value?.FirstOrDefault();
        if (user is null)
            return null;

        if (user.AdditionalData.TryGetValue(_officeNameKey, out var value)
            && value is string officeName
            && !string.IsNullOrWhiteSpace(officeName))
        {
            return officeName;
        }

        return null;
    }
}
