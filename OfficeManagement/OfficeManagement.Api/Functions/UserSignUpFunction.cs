using Microsoft.AspNetCore.Http;
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
    public async Task<HttpResponseData> Run(HttpRequestData req, FunctionContext executionContext)
    {
        // 1. Parse Entra Request
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(requestBody);

        // 2. Extract User Input (The "Invitation Code")
        // Note: The actual JSON path depends on your extension App ID
        var appId = _configuration["EntraAppId"];
        var userCode = data.GetProperty("data").GetProperty("userSignUpInfo")
                           .GetProperty("attributes").GetProperty($"extension_{appId}_InvitationCode").GetString();

        // 3. Validate against your DB (Pseudo-code)
        // const string VALID_CODE = "LAW-2026-X"; 
        var testIsValid = Convert.ToBoolean(_configuration["TestIsValid"]);
        bool isValid = testIsValid; //CheckDatabaseForCode(userCode);

        var response = req.CreateResponse(HttpStatusCode.OK);

        if (!isValid)
        {
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
        return response;
    }
}