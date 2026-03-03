using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text.Json;

namespace OfficeManagement.Api.Functions;

public class UserSignInFunction(ILogger<UserSignInFunction> logger,
    ILawyerService lawyerService)
{
    private readonly ILogger<UserSignInFunction> _logger = logger;
    private readonly ILawyerService _lawyerService = lawyerService;

    [Function("UserSignInFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "usersignin")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        const int maxLoggedPayloadLength = 8000;
        var loggedPayload = requestBody.Length > maxLoggedPayloadLength
            ? $"{requestBody[..maxLoggedPayloadLength]}... [truncated]"
            : requestBody;
        _logger.LogWarning("Incoming TokenIssuanceStart payload: {Payload}", loggedPayload);


        JsonElement root;
        try
        {
            root = JsonSerializer.Deserialize<JsonElement>(requestBody);
        }
        catch
        {
            return new BadRequestObjectResult(new { error = "Invalid request payload." });
        }

        _logger.LogWarning("Passed JsonSerializer.Deserialize");

        if (!root.TryGetProperty("data", out var calloutData)
            || !calloutData.TryGetProperty("authenticationContext", out _))
        {
            return new BadRequestObjectResult(new { error = "Invalid TokenIssuanceStart payload shape." });
        }

        _logger.LogWarning("Passed getting data");

        var userEmail = GetUserEmail(calloutData);

        _logger.LogWarning("Passed getting user email {UserEmail}", userEmail);

        if (!string.IsNullOrWhiteSpace(userEmail))
        {
            var lawyer = await _lawyerService.GetByEmail(userEmail);

            if (lawyer is null)
            {
                _logger.LogWarning("Passed getting lawyer, and it's null");
                return BuildBlockResponse("User with this email doesn't exist. Please contact your administrator.");
            }

            _logger.LogWarning("Passed getting lawyer with officeId {OfficeId}", lawyer.OfficeId);

            return BuildContinueResponse(lawyer.OfficeId);
        }

        _logger.LogWarning("The user email is invalid oh no");

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