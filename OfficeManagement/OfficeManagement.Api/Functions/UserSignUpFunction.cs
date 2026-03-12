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
/// Handles Entra sign-up attribute collection callouts.
/// </summary>
public class UserSignUpFunction(ILogger<UserSignUpFunction> logger,
    IConfiguration configuration,
    ILawyerService lawyerService,
    IOfficeService officeService)
{
    private readonly ILogger<UserSignUpFunction> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILawyerService _lawyerService = lawyerService;
    private readonly IOfficeService _officeService = officeService;

    /// <summary>
    /// Validates invitation code or registers a new office during sign-up flow.
    /// </summary>
    [Function("UserSignUpFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "usersignup")] HttpRequest req)
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
            || !calloutData.TryGetProperty("userSignUpInfo", out var userSignUpInfo)
            || !userSignUpInfo.TryGetProperty("attributes", out var attributes))
        {
            return new BadRequestObjectResult(new { error = "Invalid AttributeCollectionSubmit payload shape." });
        }

        var appId = (_configuration["EntraAppId"] ?? string.Empty).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
        var invitationKey = $"extension_{appId}_InvitationCode";
        var officeKey = $"extension_{appId}_OfficeName";

        var userEmail = GetUserEmail(userSignUpInfo);
        var userInvitationCode = GetAttributeValue(attributes, invitationKey);
        var officeName = GetAttributeValue(attributes, officeKey);
        var userDisplayName = GetAttributeValue(attributes, "displayName");

        if (!string.IsNullOrWhiteSpace(userInvitationCode) && !string.IsNullOrWhiteSpace(userEmail))
        {
            bool isValid = await _lawyerService.ValidateInvitationCode(userEmail, userInvitationCode);

            if (!isValid)
            {
                return BuildBlockResponse("This invitation code is invalid. Please contact your administrator.");
            }

            return BuildContinueResponse();
        }

        if (!string.IsNullOrWhiteSpace(officeName) && !string.IsNullOrWhiteSpace(userEmail))
        {
            bool userWithEmailExist = await _lawyerService.UserWithEmailExist(userEmail);

            if (userWithEmailExist)
            {
                return BuildBlockResponse("Office can't be registered for a user with existing email. Please contact your administrator.");
            }

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

            return BuildContinueResponse();
        }

        return BuildBlockResponse("The invitation code or office name is invalid. Please contact your administrator.");
    }

    private static string? GetUserEmail(JsonElement userSignUpInfo)
    {
        if (!userSignUpInfo.TryGetProperty("identities", out var identities) || identities.ValueKind != JsonValueKind.Array)
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

    private static string? GetAttributeValue(JsonElement attributes, string key)
    {
        if (!attributes.TryGetProperty(key, out var attributeNode))
        {
            return null;
        }

        if (attributeNode.ValueKind == JsonValueKind.Object
            && attributeNode.TryGetProperty("value", out var valueNode)
            && valueNode.ValueKind == JsonValueKind.String)
        {
            return valueNode.GetString();
        }

        return attributeNode.ValueKind == JsonValueKind.String ? attributeNode.GetString() : null;
    }

    private static OkObjectResult BuildContinueResponse() =>
        new(new Dictionary<string, object?>
        {
            ["data"] = new Dictionary<string, object?>
            {
                ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
                ["actions"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.continueWithDefaultBehavior"
                    }
                }
            }
        });

    private static OkObjectResult BuildBlockResponse(string message) =>
        new(new Dictionary<string, object?>
        {
            ["data"] = new Dictionary<string, object?>
            {
                ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
                ["actions"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.showBlockPage",
                        ["message"] = message
                    }
                }
            }
        });
}