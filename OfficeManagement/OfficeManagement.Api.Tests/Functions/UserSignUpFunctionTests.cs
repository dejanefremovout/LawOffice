using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using OfficeManagement.Api.Functions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.ViewModels;
using System.Text;

namespace OfficeManagement.Api.Tests.Functions;

public class UserSignUpFunctionTests
{
    private readonly ILogger<UserSignUpFunction> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILawyerService _lawyerService;
    private readonly IOfficeService _officeService;
    private readonly UserSignUpFunction _sut;

    public UserSignUpFunctionTests()
    {
        _logger = Substitute.For<ILogger<UserSignUpFunction>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EntraAppId"] = "11111111-1111-1111-1111-111111111111"
            })
            .Build();
        _lawyerService = Substitute.For<ILawyerService>();
        _officeService = Substitute.For<IOfficeService>();
        _sut = new UserSignUpFunction(_logger, _configuration, _lawyerService, _officeService);
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenPayloadIsInvalidJson()
    {
        var request = CreateRequest("{invalid-json}");

        IActionResult result = await _sut.Run(request);

      result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Run_ReturnsContinue_WhenInvitationCodeIsValid()
    {
        const string payload = """
            {
              "data": {
                "userSignUpInfo": {
                  "identities": [
                    { "issuerAssignedId": "jane.doe@example.com" }
                  ],
                  "attributes": {
                    "extension_11111111111111111111111111111111_InvitationCode": {
                      "value": "INVITE-123"
                    }
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.ValidateInvitationCode("jane.doe@example.com", "INVITE-123").Returns(true);

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Run_ReturnsBlock_WhenOfficeNameProvidedAndEmailExists()
    {
        const string payload = """
            {
              "data": {
                "userSignUpInfo": {
                  "identities": [
                    { "issuerAssignedId": "jane.doe@example.com" }
                  ],
                  "attributes": {
                    "extension_11111111111111111111111111111111_OfficeName": {
                      "value": "Acme Office"
                    }
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.UserWithEmailExist("jane.doe@example.com").Returns(true);

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
        await _officeService.DidNotReceive().Create(Arg.Any<OfficeCreateModel>());
    }

    [Fact]
    public async Task Run_ReturnsContinueWithOfficeName_WhenOfficeRegistrationIsValid()
    {
        const string payload = """
            {
              "data": {
                "userSignUpInfo": {
                  "identities": [
                    { "issuerAssignedId": "jane.doe@example.com" }
                  ],
                  "attributes": {
                    "displayName": { "value": "Jane Doe" },
                    "extension_11111111111111111111111111111111_OfficeName": {
                      "value": "Acme Office"
                    }
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.UserWithEmailExist("jane.doe@example.com").Returns(false);

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
        await _officeService.DidNotReceive().Create(Arg.Any<OfficeCreateModel>());
        await _lawyerService.DidNotReceive().Create(Arg.Any<LawyerCreateModel>());
    }

    private static HttpRequest CreateRequest(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return context.Request;
    }
}
