using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text.Json;
using System.Xml.Linq;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// Handles Entra token issuance sign-in callout validation.
/// </summary>
public class UserSignInFunction(ILogger<UserSignInFunction> logger,
    IConfiguration configuration,
    ILawyerService lawyerService,
    IOfficeService officeService)
{
    private readonly ILogger<UserSignInFunction> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILawyerService _lawyerService = lawyerService;
    private readonly IOfficeService _officeService = officeService;

    /// <summary>
    /// Validates the incoming sign-in payload and enriches token claims when allowed.
    /// </summary>
    [Function("UserSignInFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "usersignin")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        JsonElement root;
        try
        {
            root = JsonSerializer.Deserialize<JsonElement>(requestBody);
        }
        catch
        {
            return new BadRequestObjectResult(new { error = "Invalid request payload." });
        }

        if (!root.TryGetProperty("data", out var calloutData)
            || !calloutData.TryGetProperty("authenticationContext", out _))
        {
            return new BadRequestObjectResult(new { error = "Invalid TokenIssuanceStart payload shape." });
        }

        var appId = (_configuration["EntraAppId"] ?? string.Empty).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        var officeKey = $"extension_{appId}_OfficeName";

        var userEmail = GetUserEmail(calloutData);

        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            var lawyer = await _lawyerService.GetByEmail(userEmail);

            if (lawyer is null)
            {
                var officeName = GetExtensionAttribute(calloutData, officeKey);

                if (officeName is null)
                {
                    return BuildBlockResponse("User with this email doesn't exist. Please contact your administrator.");
                }

                var userDisplayName = GetExtensionAttribute(calloutData, "displayName");

                var officeModel = new OfficeCreateModel()
                {
                    Name = officeName
                };

                OfficeModel officeResult = await _officeService.Create(officeModel);

                LawyerCreateModel lawyerModel = new LawyerCreateModel()
                {
                    FirstName = userDisplayName ?? userEmail,
                    LastName = userDisplayName ?? userEmail,
                    Email = userEmail,
                    OfficeId = officeResult.Id
                };
                _ = await _lawyerService.Create(lawyerModel);

                return BuildContinueResponse(officeResult.Id);
            }

            return BuildContinueResponse(lawyer.OfficeId);
        }

        return BuildBlockResponse("The user email is invalid. Please contact your administrator.");
    }

    private static string? GetUserEmail(JsonElement calloutData)
    {
        if (calloutData.TryGetProperty("authenticationContext", out var authenticationContext)
            && authenticationContext.ValueKind == JsonValueKind.Object
            && authenticationContext.TryGetProperty("user", out var contextUser)
            && contextUser.ValueKind == JsonValueKind.Object)
        {
            if (contextUser.TryGetProperty("mail", out var mail)
                && mail.ValueKind == JsonValueKind.String)
            {
                return mail.GetString();
            }

            if (contextUser.TryGetProperty("userPrincipalName", out var userPrincipalName)
                && userPrincipalName.ValueKind == JsonValueKind.String)
            {
                return userPrincipalName.GetString();
            }
        }

        if (calloutData.TryGetProperty("user", out var user)
            && user.ValueKind == JsonValueKind.Object)
        {
            if (user.TryGetProperty("mail", out var mail)
                && mail.ValueKind == JsonValueKind.String)
            {
                return mail.GetString();
            }

            if (user.TryGetProperty("userPrincipalName", out var userPrincipalName)
                && userPrincipalName.ValueKind == JsonValueKind.String)
            {
                return userPrincipalName.GetString();
            }
        }

        if (!calloutData.TryGetProperty("userSignInInfo", out var userSignInInfo)
            || !userSignInInfo.TryGetProperty("identities", out var identities)
            || identities.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var identity in identities.EnumerateArray())
        {
            if (identity.TryGetProperty("issuerAssignedId", out var issuerAssignedId)
                && issuerAssignedId.ValueKind == JsonValueKind.String)
            {
                return issuerAssignedId.GetString();
            }
        }

        return null;
    }

    private static string? GetExtensionAttribute(JsonElement calloutData, string key)
    {
        if (calloutData.TryGetProperty("authenticationContext", out var authenticationContext)
            && authenticationContext.ValueKind == JsonValueKind.Object
            && authenticationContext.TryGetProperty("user", out var user)
            && user.ValueKind == JsonValueKind.Object
            && user.TryGetProperty(key, out var attributeNode)
            && attributeNode.ValueKind == JsonValueKind.String)
        {
            return attributeNode.GetString();
        }

        return null;
    }

    private static OkObjectResult BuildContinueResponse(string officeId) =>
        new(new Dictionary<string, object?>
        {
            ["data"] = new Dictionary<string, object?>
            {
                ["@odata.type"] = "microsoft.graph.onTokenIssuanceStartResponseData",
                ["actions"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@odata.type"] = "microsoft.graph.tokenIssuanceStart.provideClaimsForToken",
                        ["claims"] = new Dictionary<string, object?>
                        {
                            ["officeId"] = officeId
                        }
                    }
                }
            }
        });

    private static ObjectResult BuildBlockResponse(string message) =>
        new(new Dictionary<string, object?>
        {
            ["error"] = new Dictionary<string, object?>
            {
                ["code"] = "access_denied",
                ["message"] = message
            }
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
}