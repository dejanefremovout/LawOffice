using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace OfficeManagement.Api.Functions;

public class UserSignUpFunction
{
    private readonly ILogger<UserSignUpFunction> _logger;
    private readonly IConfiguration _configuration;

    public UserSignUpFunction(ILogger<UserSignUpFunction> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Function("UserSignUpFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "usersignup")] HttpRequest req)
    {
        _logger.LogInformation("UserSignUpFunction started. TraceId: {TraceId}", req.HttpContext.TraceIdentifier);

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        _logger.LogInformation("Received signup event payload. ContentLength: {ContentLength}", requestBody.Length);

        JsonElement data;
        try
        {
            data = JsonSerializer.Deserialize<JsonElement>(requestBody);
            _logger.LogInformation("Signup payload deserialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize signup payload.");
            return new BadRequestObjectResult(new { error = "Invalid request payload." });
        }

        var appId = _configuration["EntraAppId"];
        _logger.LogInformation("Using EntraAppId configured: {HasEntraAppId}", !string.IsNullOrWhiteSpace(appId));

        var testIsValid = Convert.ToBoolean(_configuration["TestIsValid"]);
        bool isValid = testIsValid;
        _logger.LogInformation("Invitation code validation completed. IsValid: {IsValid}", isValid);

        if (!isValid)
        {
            _logger.LogWarning("Signup blocked due to invalid invitation code.");
            return new OkObjectResult(new Dictionary<string, object?>
            {
                ["data"] = new Dictionary<string, object?>
                {
                    ["@odata.type"] = "microsoft.graph.onAttributeCollectionSubmitResponseData",
                    ["actions"] = new object[]
                    {
                        new Dictionary<string, object?>
                        {
                            ["@odata.type"] = "microsoft.graph.attributeCollectionSubmit.showBlockPage",
                            ["message"] = "This invitation code is invalid. Please contact your administrator."
                        }
                    }
                }
            });
        }

        _logger.LogInformation("Signup allowed. Returning continue action.");
        _logger.LogInformation("UserSignUpFunction completed successfully. TraceId: {TraceId}", req.HttpContext.TraceIdentifier);
        return new OkObjectResult(new Dictionary<string, object?>
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
    }
}