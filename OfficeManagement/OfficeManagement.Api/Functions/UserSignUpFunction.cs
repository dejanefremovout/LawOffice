using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
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
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, Route = "usersignup")] HttpRequestData req, FunctionContext executionContext)
    {
        _logger.LogInformation("UserSignUpFunction started. InvocationId: {InvocationId}", executionContext.InvocationId);

        // 1. Parse Entra Request
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
            var badRequest = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequest.WriteAsJsonAsync(new { error = "Invalid request payload." });
            return badRequest;
        }

        // 2. Extract User Input (The "Invitation Code")
        // Note: The actual JSON path depends on your extension App ID
        var appId = _configuration["EntraAppId"];
        _logger.LogInformation("Using EntraAppId configured: {HasEntraAppId}", !string.IsNullOrWhiteSpace(appId));
        //var userCode = data.GetProperty("data").GetProperty("userSignUpInfo")
        //                   .GetProperty("attributes").GetProperty($"extension_{appId}_InvitationCode").GetString();
        //var userInvitationCode = data.GetProperty("attrs").GetProperty($"extension_{appId}_InvitationCode").GetString();

        // 3. Validate against your DB (Pseudo-code)
        // const string VALID_CODE = "LAW-2026-X";
        var testIsValid = Convert.ToBoolean(_configuration["TestIsValid"]);
        bool isValid = testIsValid; //CheckDatabaseForCode(userCode);
        _logger.LogInformation("Invitation code validation completed. IsValid: {IsValid}", isValid);

        var response = req.CreateResponse(HttpStatusCode.OK);

        if (!isValid)
        {
            _logger.LogWarning("Signup blocked due to invalid invitation code.");
            // BLOCK THE USER
            await response.WriteAsJsonAsync(new
            {
                data = new
                {
                    actions = new[] {
                    new {
                        type = "showBlockPage",
                        message = "This invitation code is invalid. Please contact your administrator."
                    }
                }
                }
            });
        }
        else
        {
            _logger.LogInformation("Signup allowed. Returning continue action.");
            // ALLOW THE USER
            await response.WriteAsJsonAsync(new
            {
                data = new
                {
                    actions = new[] {
                    new { type = "continue" }
                }
                }
            });
        }

        _logger.LogInformation("UserSignUpFunction completed successfully. InvocationId: {InvocationId}", executionContext.InvocationId);
        return response;
    }
}