using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeManagement.Application.Services;
using System.Text.Json;

namespace OfficeManagement.Api.Functions;

/// <summary>
/// Handles Entra sign-up attribute collection callouts.
/// </summary>
public class UserSignUpFunction(ILogger<UserSignUpFunction> logger,
    IConfiguration configuration,
    ILawyerService lawyerService)
{
    private readonly ILogger<UserSignUpFunction> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILawyerService _lawyerService = lawyerService;

    /// <summary>
    /// Validates invitation code or registers a new office during sign-up flow.
    /// </summary>
    [Function("UserSignUpFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "usersignup")] HttpRequest req)
    {
        _logger.LogWarning("UserSignUpFunction triggered. Reading request body.");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        _logger.LogWarning("UserSignUpFunction received request body of length {BodyLength}.", requestBody.Length);

        JsonElement root;
        try
        {
            root = JsonSerializer.Deserialize<JsonElement>(requestBody);
            _logger.LogWarning("UserSignUpFunction successfully deserialized request body.");
        }
        catch
        {
            _logger.LogWarning("UserSignUpFunction failed to deserialize request body. Returning BadRequest.");
            return new BadRequestObjectResult(new { error = "Invalid request payload." });
        }

        if (!root.TryGetProperty("data", out var calloutData)
            || !calloutData.TryGetProperty("userSignUpInfo", out var userSignUpInfo)
            || !userSignUpInfo.TryGetProperty("attributes", out var attributes))
        {
            _logger.LogWarning("UserSignUpFunction payload is missing required properties: data, userSignUpInfo, or attributes. Returning BadRequest.");
            return new BadRequestObjectResult(new { error = "Invalid AttributeCollectionSubmit payload shape." });
        }

        _logger.LogWarning("UserSignUpFunction payload shape is valid. Resolving extension keys using EntraAppId.");

        var appId = (_configuration["EntraAppId"] ?? string.Empty).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        _logger.LogWarning("UserSignUpFunction resolved appId (length {AppIdLength}).", appId.Length);

        var invitationKey = $"extension_{appId}_InvitationCode";
        var officeKey = $"extension_{appId}_OfficeName";

        _logger.LogWarning("UserSignUpFunction resolved invitationKey={InvitationKey}, officeKey={OfficeKey}.", invitationKey, officeKey);

        var userEmail = GetUserEmail(userSignUpInfo);
        var userInvitationCode = GetAttributeValue(attributes, invitationKey);
        var officeName = GetAttributeValue(attributes, officeKey);

        _logger.LogWarning("UserSignUpFunction extracted userEmail={UserEmail}, hasInvitationCode={HasInvitationCode}, hasOfficeName={HasOfficeName}.",
            userEmail, !string.IsNullOrWhiteSpace(userInvitationCode), !string.IsNullOrWhiteSpace(officeName));

        if (!string.IsNullOrWhiteSpace(userInvitationCode) && !string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogWarning("UserSignUpFunction entering invitation code validation path for userEmail={UserEmail}.", userEmail);

            bool isValid = await _lawyerService.ValidateInvitationCode(userEmail, userInvitationCode);

            _logger.LogWarning("UserSignUpFunction invitation code validation result for userEmail={UserEmail}: isValid={IsValid}.", userEmail, isValid);

            if (!isValid)
            {
                _logger.LogWarning("UserSignUpFunction invitation code is invalid for userEmail={UserEmail}. Returning block response.", userEmail);
                return BuildBlockResponse("This invitation code is invalid. Please contact your administrator.");
            }

            _logger.LogWarning("UserSignUpFunction invitation code is valid for userEmail={UserEmail}. Returning continue response.", userEmail);
            return BuildContinueResponse();
        }

        if (!string.IsNullOrWhiteSpace(officeName) && !string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogWarning("UserSignUpFunction entering office registration path for userEmail={UserEmail}, officeName={OfficeName}.", userEmail, officeName);

            bool userWithEmailExist = await _lawyerService.UserWithEmailExist(userEmail);

            _logger.LogWarning("UserSignUpFunction email existence check for userEmail={UserEmail}: userWithEmailExist={UserWithEmailExist}.", userEmail, userWithEmailExist);

            if (userWithEmailExist)
            {
                _logger.LogWarning("UserSignUpFunction office registration blocked: userEmail={UserEmail} already exists. Returning block response.", userEmail);
                return BuildBlockResponse("Office can't be registered for a user with existing email. Please contact your administrator.");
            }

            _logger.LogWarning("UserSignUpFunction office registration allowed for userEmail={UserEmail}, officeName={OfficeName}. Returning continue response with office name.", userEmail, officeName);
            return BuildContinueResponse();
        }

        _logger.LogWarning("UserSignUpFunction no valid path matched: userEmail={UserEmail}, hasInvitationCode={HasInvitationCode}, hasOfficeName={HasOfficeName}. Returning block response.",
            userEmail, !string.IsNullOrWhiteSpace(userInvitationCode), !string.IsNullOrWhiteSpace(officeName));

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

    //private static OkObjectResult BuildContinueResponseWithOfficeName(string officeKey, string officeName) =>
    //    new(new Dictionary<string, object?>
    //    {
    //        ["data"] = new Dictionary<string, object?>
    //        {
    //            ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
    //            ["actions"] = new object[]
    //            {
    //                new Dictionary<string, object?>
    //                {
    //                    ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.modifyAttributeValues",
    //                    ["attributes"] = new object[]
    //                    {
    //                        new Dictionary<string, object?>
    //                        {
    //                            ["name"] = officeKey,
    //                            ["value"] = officeName
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    });

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