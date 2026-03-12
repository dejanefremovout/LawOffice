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

public class UserSignInFunctionTests
{
    private readonly ILogger<UserSignInFunction> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILawyerService _lawyerService;
    private readonly IOfficeService _officeService;
    private readonly UserSignInFunction _sut;

    public UserSignInFunctionTests()
    {
        _logger = Substitute.For<ILogger<UserSignInFunction>>();
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["EntraAppId"] = "11111111-1111-1111-1111-111111111111"
            })
            .Build();
        _lawyerService = Substitute.For<ILawyerService>();
        _officeService = Substitute.For<IOfficeService>();
        _sut = new UserSignInFunction(_logger, _configuration, _lawyerService, _officeService);
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenPayloadIsInvalidJson()
    {
        var request = CreateRequest("{invalid-json}");

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenAuthenticationContextIsMissing()
    {
        const string payload = """
            {
              "data": {
                "someOtherField": {}
              }
            }
            """;

        var request = CreateRequest(payload);

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Run_ReturnsForbidden_WhenEmailCannotBeResolved()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {}
              }
            }
            """;

        var request = CreateRequest(payload);

        IActionResult result = await _sut.Run(request);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task Run_ReturnsForbidden_WhenUserDoesNotExist()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {},
                "user": { "mail": "missing@example.com" }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("missing@example.com").Returns((LawyerModel?)null);

        IActionResult result = await _sut.Run(request);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task Run_ReturnsForbidden_WhenLawyerNotFoundAndOfficeNameMissing()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {
                  "user": {
                    "mail": "new@example.com"
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("new@example.com").Returns((LawyerModel?)null);

        IActionResult result = await _sut.Run(request);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
        await _officeService.DidNotReceive().Create(Arg.Any<OfficeCreateModel>());
        await _lawyerService.DidNotReceive().Create(Arg.Any<LawyerCreateModel>());
    }

    [Fact]
    public async Task Run_CreatesOfficeAndLawyerAndReturnsContinue_WhenFirstSignInWithOfficeName()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {
                  "user": {
                    "mail": "jane.doe@example.com",
                    "extension_11111111111111111111111111111111_OfficeName": "Acme Office",
                    "displayName": "Jane Doe"
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("jane.doe@example.com").Returns((LawyerModel?)null);
        _officeService.Create(Arg.Any<OfficeCreateModel>())
            .Returns(new OfficeModel { Id = "office-1", Name = "Acme Office" });
        _lawyerService.Create(Arg.Any<LawyerCreateModel>())
            .Returns(new LawyerModel
            {
                Id = "lawyer-1",
                OfficeId = "office-1",
                FirstName = "Jane Doe",
                LastName = "Jane Doe",
                Email = "jane.doe@example.com"
            });

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
        await _officeService.Received(1).Create(Arg.Is<OfficeCreateModel>(x => x.Name == "Acme Office"));
        await _lawyerService.Received(1).Create(Arg.Is<LawyerCreateModel>(x =>
            x.Email == "jane.doe@example.com" &&
            x.FirstName == "Jane Doe" &&
            x.LastName == "Jane Doe" &&
            x.OfficeId == "office-1"));
    }

    [Fact]
    public async Task Run_UsesEmailAsDisplayName_WhenDisplayNameNotPresent()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {
                  "user": {
                    "mail": "jane.doe@example.com",
                    "extension_11111111111111111111111111111111_OfficeName": "Acme Office"
                  }
                }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("jane.doe@example.com").Returns((LawyerModel?)null);
        _officeService.Create(Arg.Any<OfficeCreateModel>())
            .Returns(new OfficeModel { Id = "office-1", Name = "Acme Office" });
        _lawyerService.Create(Arg.Any<LawyerCreateModel>())
            .Returns(new LawyerModel
            {
                Id = "lawyer-1",
                OfficeId = "office-1",
                FirstName = "jane.doe@example.com",
                LastName = "jane.doe@example.com",
                Email = "jane.doe@example.com"
            });

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
        await _lawyerService.Received(1).Create(Arg.Is<LawyerCreateModel>(x =>
            x.FirstName == "jane.doe@example.com" &&
            x.LastName == "jane.doe@example.com"));
    }

    [Fact]
    public async Task Run_ReturnsOk_WhenUserExists()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {},
                "user": { "mail": "jane.doe@example.com" }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("jane.doe@example.com")
            .Returns(new LawyerModel
            {
                Id = "lawyer-1",
                OfficeId = "office-1",
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com"
            });

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
    }

    private static HttpRequest CreateRequest(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return context.Request;
    }
}
