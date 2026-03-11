using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text.Json;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// Handles Entra token issuance sign-in callout validation.
/// </summary>
public class UserSignInFunction(ILogger<UserSignInFunction> logger,
    ILawyerService lawyerService)
{
    private readonly ILogger<UserSignInFunction> _logger = logger;
    private readonly ILawyerService _lawyerService = lawyerService;

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

        var userEmail = GetUserEmail(calloutData);

        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            var lawyer = await _lawyerService.GetByEmail(userEmail);

            if (lawyer is null)
            {
                return BuildBlockResponse("User with this email doesn't exist. Please contact your administrator.");
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