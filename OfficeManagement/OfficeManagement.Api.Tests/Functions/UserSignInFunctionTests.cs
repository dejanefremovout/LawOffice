using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using OfficeManagement.Api.Functions;
using OfficeManagement.Application.Services;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Domain.ViewModels;
using System.Text;

namespace OfficeManagement.Api.Tests.Functions;

public class UserSignInFunctionTests
{
    private readonly ILawyerService _lawyerService;
    private readonly IOfficeService _officeService;
    private readonly IGraphUserService _graphUserService;
    private readonly UserSignInFunction _sut;

    public UserSignInFunctionTests()
    {
        _lawyerService = Substitute.For<ILawyerService>();
        _officeService = Substitute.For<IOfficeService>();
        _graphUserService = Substitute.For<IGraphUserService>();
        _sut = new UserSignInFunction(_lawyerService, _officeService, _graphUserService);
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
    public async Task Run_ReturnsForbidden_WhenUserDoesNotExistAndGraphReturnsNoOfficeName()
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
        _graphUserService.GetOfficeName("missing@example.com").Returns((string?)null);

        IActionResult result = await _sut.Run(request);

        var objectResult = result.ShouldBeOfType<ObjectResult>();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
    }

    [Fact]
    public async Task Run_CreatesOfficeAndLawyer_OnFirstSignIn()
    {
        const string payload = """
            {
              "data": {
                "authenticationContext": {
                  "user": {
                    "mail": "new.user@example.com",
                    "displayName": "New User"
                  }
                },
                "user": { "mail": "new.user@example.com", "displayName": "New User" }
              }
            }
            """;

        var request = CreateRequest(payload);
        _lawyerService.GetByEmail("new.user@example.com").Returns((LawyerModel?)null);
        _graphUserService.GetOfficeName("new.user@example.com").Returns("New Office");
        _officeService.Create(Arg.Any<OfficeCreateModel>())
            .Returns(new OfficeModel { Id = "office-new", Name = "New Office" });
        _lawyerService.Create(Arg.Any<LawyerCreateModel>())
            .Returns(new LawyerModel
            {
                Id = "lawyer-new",
                OfficeId = "office-new",
                FirstName = "New User",
                LastName = "New User",
                Email = "new.user@example.com"
            });

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<OkObjectResult>();
        await _officeService.Received(1).Create(Arg.Is<OfficeCreateModel>(x => x.Name == "New Office"));
        await _lawyerService.Received(1).Create(Arg.Is<LawyerCreateModel>(x =>
            x.Email == "new.user@example.com" &&
            x.OfficeId == "office-new"));
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
