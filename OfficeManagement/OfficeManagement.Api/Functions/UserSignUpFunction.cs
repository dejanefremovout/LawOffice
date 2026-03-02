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

        JsonElement data;
        try
        {
            data = JsonSerializer.Deserialize<JsonElement>(requestBody);
        }
        catch
        {
            return new BadRequestObjectResult(new { error = "Invalid request payload." });
        }

        var appId = _configuration["EntraAppId"];
        var userEmail = data.GetProperty("attrs").GetProperty($"extension_{appId}_InvitationCode").GetString();
        var userInvitationCode = data.GetProperty("attrs").GetProperty($"extension_{appId}_InvitationCode").GetString();
        
        if (!string.IsNullOrWhiteSpace(userInvitationCode))
        {
            bool isValid = await _lawyerService.ValidateInvitationCode(userEmail!, userInvitationCode!);

            if (!isValid)
            {
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

        var officeName = data.GetProperty("attrs").GetProperty($"extension_{appId}_OfficeName").GetString();
        var userDisplayName = data.GetProperty("attrs").GetProperty("displayName").GetString();

        if (!string.IsNullOrWhiteSpace(officeName))
        {
            var officeModel = new OfficeCreateModel()
            { 
                Name = officeName 
            };

            OfficeModel officeResult = await _officeService.Create(officeModel);

            LawyerCreateModel lawyerModel = new LawyerCreateModel()
            {
                FirstName = userDisplayName!,
                LastName = userDisplayName!,
                Email = userEmail!,
                OfficeId = officeResult.Id
            };
            LawyerModel lawyerResult = await _lawyerService.Create(lawyerModel);

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
                            ["message"] = "The invitation code or office name is invalid. Please contact your administrator."
                        }
                        }
            }
        });
    }
}