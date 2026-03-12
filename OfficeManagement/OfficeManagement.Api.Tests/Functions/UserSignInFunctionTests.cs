using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    private readonly ILawyerService _lawyerService;
    private readonly UserSignInFunction _sut;

    public UserSignInFunctionTests()
    {
        _logger = Substitute.For<ILogger<UserSignInFunction>>();
        _lawyerService = Substitute.For<ILawyerService>();
        _sut = new UserSignInFunction(_logger, _lawyerService);
    }

    [Fact]
    public async Task Run_ReturnsBadRequest_WhenPayloadIsInvalidJson()
    {
        var request = CreateRequest("{invalid-json}");

        IActionResult result = await _sut.Run(request);

        result.ShouldBeOfType<BadRequestObjectResult>();
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
