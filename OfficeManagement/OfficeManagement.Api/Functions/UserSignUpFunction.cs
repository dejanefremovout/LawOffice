using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text.Json;

namespace OfficeManagement.Api.Functions;

public class UserSignUpFunction(ILogger<UserSignUpFunction> logger,
    IConfiguration configuration,
    ILawyerService lawyerService,
    IOfficeService officeService)
{
    private readonly ILogger<UserSignUpFunction> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILawyerService _lawyerService = lawyerService;
    private readonly IOfficeService _officeService = officeService;

    [Function("UserSignUpFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "usersignup")] HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        const int maxLoggedPayloadLength = 8000;
        var loggedPayload = requestBody.Length > maxLoggedPayloadLength
            ? $"{requestBody[..maxLoggedPayloadLength]}... [truncated]"
            : requestBody;
        _logger.LogTrace("Incoming AttributeCollectionSubmit payload: {Payload}", loggedPayload);

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
            var validity = await _lawyerService.ValidateInvitationCode(userEmail, userInvitationCode);
            bool isValid = validity.Item1;
            var lawyerCode = validity.Item2.InvitationCode;

            _logger.LogWarning("User {UserEmail} is signing up with invitation code {UserInvitationCode}. Working code: {LawyerCode}", userEmail, userInvitationCode, lawyerCode);

            if (!isValid)
            {
                return BuildBlockResponse("This invitation code is invalid. Please contact your administrator.");
            }

            return BuildContinueResponse();
        }

        if (!string.IsNullOrWhiteSpace(officeName) && !string.IsNullOrWhiteSpace(userEmail))
        {
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